using System;
using System.ComponentModel.DataAnnotations;
using BL.PersonalInfo;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL.PersonalInfo;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests;

public class PersonalInfoManagerTests
{
    private readonly Mock<IPersonalInfoRepository> _repoMock;
    private readonly PersonalInfoManager _manager;

    public PersonalInfoManagerTests()
    {
        _repoMock = new Mock<IPersonalInfoRepository>();
        _manager = new PersonalInfoManager(_repoMock.Object);
    }

    [Fact]
    public void Add_ShouldUpdateExistingPerson_WhenIdExists()
    {
        var existingPerson = new Person
        {
            Id = 1,
            FirstName = "Old",
            LastName = "User",
            Street = "Old Street",
            HouseNumber = "1",
            PostalCode = "1000",
            City = "Brussel",
            Country = "Belgie",
            Email = "old@example.com"
        };

        _repoMock.Setup(r => r.GetById(1)).Returns(existingPerson);
        _repoMock.Setup(r => r.Update(existingPerson)).Returns(existingPerson);

        var result = _manager.Add(1, "Said", "Khalaf", null, Gender.Male, MaritalStatus.Single,
            "Nieuwe Straat", "10", "", "2000", "Antwerpen", "Belgie",
            "said@example.com", "0470123456", "https://linkedin.com/in/said", "Consultant", "I work as a consultant in this company");

        result.Id.Should().Be(1);
        result.FirstName.Should().Be("Said");
        result.City.Should().Be("Antwerpen");

        _repoMock.Verify(r => r.Update(existingPerson), Times.Once);
        _repoMock.Verify(r => r.Create(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public void Add_ShouldCreatePerson_WhenIdDoesNotExist()
    {
        Person createdPerson = null!;

        _repoMock.Setup(r => r.GetById(1)).Returns((Person)null!);
        _repoMock
            .Setup(r => r.Create(It.IsAny<Person>()))
            .Callback<Person>(p => createdPerson = p)
            .Returns<Person>(p => p);

        var result = _manager.Add(1, "Said", "Khalaf", null, Gender.Male, MaritalStatus.Single,
            "Nieuwe Straat", "10", "", "2000", "Antwerpen", "Belgie",
            "said@example.com", "0470123456", "https://linkedin.com/in/said", "Consultant", "I work as a consultant in this company");

        result.Id.Should().Be(1);
        createdPerson.Should().NotBeNull();
        createdPerson.Id.Should().Be(1);

        _repoMock.Verify(r => r.Create(It.IsAny<Person>()), Times.Once);
        _repoMock.Verify(r => r.Update(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public void Add_ShouldFallbackToDefaultId_WhenProvidedIdIsInvalid()
    {
        _repoMock.Setup(r => r.GetById(AppDefaults.DefaultPersonId)).Returns((Person)null!);
        _repoMock.Setup(r => r.Create(It.IsAny<Person>())).Returns<Person>(p => p);

        var result = _manager.Add(0, "Said", "Khalaf", null, Gender.Male, MaritalStatus.Single,
            "Nieuwe Straat", "10", "", "2000", "Antwerpen", "Belgie",
            "said@example.com", "0470123456", "https://linkedin.com/in/said", "Consultant", "I work as a consultant in this company");

        result.Id.Should().Be(AppDefaults.DefaultPersonId);
        _repoMock.Verify(r => r.GetById(AppDefaults.DefaultPersonId), Times.Once);
    }
    
    [Fact]
    public void Add_ShouldTrimInputValues_WhenSavingPerson()
    {
        Person createdPerson = null!;

        _repoMock.Setup(r => r.GetById(1)).Returns((Person)null!);
        _repoMock.Setup(r => r.Create(It.IsAny<Person>()))
            .Callback<Person>(p => createdPerson = p)
            .Returns<Person>(p => p);

        _manager.Add(1, "  Said  ", "  Khalaf ", new DateTime(1996, 6, 15), Gender.Male, MaritalStatus.Single,
            "  Meir  ", " 12 ", "  B2 ", " 2000 ", "  Antwerpen ", " Belgie  ",
            "  said.khalaf@is4u.be  ", " 0471 12 34 56 ", " https://linkedin.com/in/saidkhalaf ", "Consultant", "I work as a consultant in this company");

        createdPerson.FirstName.Should().Be("Said");
        createdPerson.LastName.Should().Be("Khalaf");
        createdPerson.Street.Should().Be("Meir");
        createdPerson.HouseNumber.Should().Be("12");
        createdPerson.Bus.Should().Be("B2");
        createdPerson.PostalCode.Should().Be("2000");
        createdPerson.City.Should().Be("Antwerpen");
        createdPerson.Country.Should().Be("Belgie");
        createdPerson.Email.Should().Be("said.khalaf@is4u.be");
    }

    [Fact]
    public void Add_ShouldUseDefaultEnums_WhenGenderOrMaritalStatusIsNull()
    {
        Person createdPerson = null!;

        _repoMock.Setup(r => r.GetById(1)).Returns((Person)null!);
        _repoMock.Setup(r => r.Create(It.IsAny<Person>()))
            .Callback<Person>(p => createdPerson = p)
            .Returns<Person>(p => p);

        _manager.Add(1, "Said", "Khalaf", new DateTime(1996, 6, 15), null, null,
            "Meir", "12", "", "2000", "Antwerpen", "Belgie",
            "said@example.com", "0471123456", "https://linkedin.com/in/said", "Consultant", "I work as a consultant in this company");

        createdPerson.Gender.Should().Be(Gender.Other);
        createdPerson.MaritalStatus.Should().Be(MaritalStatus.Single);
    }

    [Fact]
    public void Add_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        _repoMock.Setup(r => r.GetById(1)).Returns((Person)null!);

        Action act = () => _manager.Add(1, "Said", "Khalaf", null, Gender.Male, MaritalStatus.Single,
            "Meir", "12", "", "2000", "Antwerpen", "Belgie",
            "invalid-email", "0471123456", "https://linkedin.com/in/said", "Consultant", "I work as a consultant in this company");

        act.Should().Throw<ValidationException>();
        _repoMock.Verify(r => r.Create(It.IsAny<Person>()), Times.Never);
        _repoMock.Verify(r => r.Update(It.IsAny<Person>()), Times.Never);
    }
}

