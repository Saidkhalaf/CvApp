using System;
using System.Collections.Generic;
using BL;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests;

public class CvEducationManagerTest
{

    private readonly Mock<ICvEducationRepository> _repoMock;
    private readonly CvEducationManager _manager;

    public CvEducationManagerTest()
    {
        _repoMock = new Mock<ICvEducationRepository>();
        _manager = new CvEducationManager(_repoMock.Object);
    }

    [Fact]
    public void AddCvEducation_ShouldCreateCvEducation_WhenEducationAndInstituteExist()
    {
        var education = CreateEducation(2, "Applied Informatics");
        var institute = CreateInstitute(3, "KDG");

        CvEducation? savedCvEducation = null;

        _repoMock.Setup(r => r.ReadEducationById(2)).Returns(education);
        _repoMock.Setup(r => r.ReadInstituteOrSchoolById(3)).Returns(institute);
        _repoMock
            .Setup(r => r.CreateCvEducation(It.IsAny<CvEducation>()))
            .Callback<CvEducation>(cv => savedCvEducation = cv)
            .Returns<CvEducation>(cv =>
            {
                cv.Id = 10;
                return cv;
            });

        var result = _manager.AddCvEducation(
            personId: 1,
            educationId: 2,
            instituteOrSchoolId: 3,
            startYear: 2020,
            endYear: 2024,
            diploma: "diploma.pdf",
            status: EducationStatus.Validated);

        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.PersonId.Should().Be(1);
        result.MajorId.Should().Be(2);
        result.InstituteOrSchoolId.Should().Be(3);
        result.StartYear.Should().Be(2020);
        result.EndYear.Should().Be(2024);
        result.Diploma.Should().Be("diploma.pdf");
        result.Status.Should().Be(EducationStatus.Validated);

        savedCvEducation.Should().NotBeNull();
        savedCvEducation!.MajorId.Should().Be(2);
        savedCvEducation.InstituteOrSchoolId.Should().Be(3);

        _repoMock.Verify(r => r.CreateCvEducation(It.IsAny<CvEducation>()), Times.Once);
    }

    [Fact]
    public void AddCvEducation_ShouldThrowException_WhenEducationDoesNotExist()
    {
        _repoMock.Setup(r => r.ReadEducationById(99)).Returns((Education)null!);

        Action act = () => _manager.AddCvEducation(
            personId: 1,
            educationId: 99,
            instituteOrSchoolId: 3,
            startYear: 2020,
            endYear: 2024,
            diploma: "",
            status: EducationStatus.InProgress);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid education id.");
    }

    [Fact]
    public void AddCvEducation_ShouldThrowException_WhenInstituteDoesNotExist()
    {
        var education = CreateEducation(2, "Applied Informatics");

        _repoMock.Setup(r => r.ReadEducationById(2)).Returns(education);
        _repoMock.Setup(r => r.ReadInstituteOrSchoolById(99)).Returns((InstituteOrSchool)null!);

        Action act = () => _manager.AddCvEducation(
            personId: 1,
            educationId: 2,
            instituteOrSchoolId: 99,
            startYear: 2020,
            endYear: 2024,
            diploma: "",
            status: EducationStatus.InProgress);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid institute or school id.");
    }

    [Fact]
    public void GetCvEducationById_ShouldReturnCvEducation()
    {
        var cvEducation = CreateCvEducation(id: 5, majorId: 2, instituteId: 3);

        _repoMock.Setup(r => r.ReadCvEducationById(5)).Returns(cvEducation);

        var result = _manager.GetCvEducationById(5);

        result.Should().NotBeNull();
        result.Id.Should().Be(5);
        result.MajorId.Should().Be(2);
        result.InstituteOrSchoolId.Should().Be(3);

        _repoMock.Verify(r => r.ReadCvEducationById(5), Times.Once);
    }

    [Fact]
    public void GetCvEducationsByPersonId_ShouldReturnCvEducationsForThatPerson()
    {
        var cvEducations = new List<CvEducation>
        {
            CreateCvEducation(id: 1, personId: 1, majorId: 2, instituteId: 3),
            CreateCvEducation(id: 2, personId: 1, majorId: 4, instituteId: 5)
        };

        _repoMock.Setup(r => r.ReadCvEducationsByPersonId(1)).Returns(cvEducations);

        var result = _manager.GetCvEducationsByPersonId(1);

        result.Should().HaveCount(2);
        _repoMock.Verify(r => r.ReadCvEducationsByPersonId(1), Times.Once);
    }

    [Fact]
    public void UpdateCvEducation_ShouldUpdateCvEducation_WhenAllIdsAreValid()
    {
        var existingCvEducation = CreateCvEducation(
            id: 7,
            personId: 1,
            majorId: 2,
            instituteId: 3,
            startYear: 2018,
            endYear: 2021,
            diploma: "old.pdf",
            status: EducationStatus.Unknown);

        var education = CreateEducation(4, "Business Engineering");
        var institute = CreateInstitute(5, "KDG");

        _repoMock.Setup(r => r.ReadCvEducationById(7)).Returns(existingCvEducation);
        _repoMock.Setup(r => r.ReadEducationById(4)).Returns(education);
        _repoMock.Setup(r => r.ReadInstituteOrSchoolById(5)).Returns(institute);

        _manager.UpdateCvEducation(
            id: 7,
            educationId: 4,
            instituteOrSchoolId: 5,
            startYear: 2021,
            endYear: 2024,
            diploma: "new.pdf",
            status: EducationStatus.Validated);

        existingCvEducation.MajorId.Should().Be(4);
        existingCvEducation.InstituteOrSchoolId.Should().Be(5);
        existingCvEducation.StartYear.Should().Be(2021);
        existingCvEducation.EndYear.Should().Be(2024);
        existingCvEducation.Diploma.Should().Be("new.pdf");
        existingCvEducation.Status.Should().Be(EducationStatus.Validated);

        _repoMock.Verify(r => r.UpdateCvEducation(existingCvEducation), Times.Once);
    }

    [Fact]
    public void UpdateCvEducation_ShouldThrowException_WhenCvEducationDoesNotExist()
    {
        _repoMock.Setup(r => r.ReadCvEducationById(99)).Returns((CvEducation)null!);

        Action act = () => _manager.UpdateCvEducation(
            id: 99,
            educationId: 2,
            instituteOrSchoolId: 3,
            startYear: 2020,
            endYear: 2024,
            diploma: "",
            status: EducationStatus.Validated);

        act.Should().Throw<ArgumentException>()
            .WithMessage("CvEducation not found.");
    }

    [Fact]
    public void UpdateCvEducation_ShouldThrowException_WhenEducationDoesNotExist()
    {
        var existingCvEducation = CreateCvEducation(id: 7, majorId: 2, instituteId: 3);

        _repoMock.Setup(r => r.ReadCvEducationById(7)).Returns(existingCvEducation);
        _repoMock.Setup(r => r.ReadEducationById(99)).Returns((Education)null!);

        Action act = () => _manager.UpdateCvEducation(
            id: 7,
            educationId: 99,
            instituteOrSchoolId: 3,
            startYear: 2020,
            endYear: 2024,
            diploma: "",
            status: EducationStatus.Validated);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid education id.");
    }

    [Fact]
    public void UpdateCvEducation_ShouldThrowException_WhenInstituteDoesNotExist()
    {
        var existingCvEducation = CreateCvEducation(id: 7, majorId: 2, instituteId: 3);
        var education = CreateEducation(2, "Applied Informatics");

        _repoMock.Setup(r => r.ReadCvEducationById(7)).Returns(existingCvEducation);
        _repoMock.Setup(r => r.ReadEducationById(2)).Returns(education);
        _repoMock.Setup(r => r.ReadInstituteOrSchoolById(99)).Returns((InstituteOrSchool)null!);

        Action act = () => _manager.UpdateCvEducation(
            id: 7,
            educationId: 2,
            instituteOrSchoolId: 99,
            startYear: 2020,
            endYear: 2024,
            diploma: "",
            status: EducationStatus.Validated);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid institute or school id.");
    }

    [Fact]
    public void RemoveCvEducation_ShouldDeleteCvEducation_WhenCvEducationExists()
    {
        var cvEducation = CreateCvEducation(id: 7, majorId: 2, instituteId: 3);

        _repoMock.Setup(r => r.ReadCvEducationById(7)).Returns(cvEducation);

        _manager.RemoveCvEducation(7);

        _repoMock.Verify(r => r.ReadCvEducationById(7), Times.Once);
        _repoMock.Verify(r => r.DeleteCvEducation(cvEducation), Times.Once);
    }

    [Fact]
    public void RemoveCvEducation_ShouldThrowException_WhenCvEducationDoesNotExist()
    {
        _repoMock.Setup(r => r.ReadCvEducationById(99)).Returns((CvEducation)null!);

        Action act = () => _manager.RemoveCvEducation(99);

        act.Should().Throw<ArgumentException>()
            .WithMessage("CvEducation not found.");
    }

    private static Education CreateEducation(long id, string name)
    {
        return new Education
        {
            Id = id,
            Name = name,
            Eqf = 6,
            IsQualified = true,
            CreatedAt = DateTime.UtcNow,
            EducationLevelId = 1,
            EducationLevel = new EducationOptions
            {
                Id = 1,
                EducationLevel = EducationLevel.ProfessioneelGerichteBachelor,
                EqfLevel = 6
            }
        };
    }

    private static InstituteOrSchool CreateInstitute(long id, string name)
    {
        return new InstituteOrSchool
        {
            Id = id,
            Name = name,
            Type = InstituteType.School,
            Email = "info@test.com",
            Website = "https://test.com",
            TelephoneNumber = "0123456789",
            Fax = "0123456789",
            BtwNumber = "BE0123456789",
            Comments = "Test institute"
        };
    }

    private static CvEducation CreateCvEducation(
        long id,
        long personId = 1,
        long majorId = 2,
        long instituteId = 3,
        int startYear = 2020,
        int endYear = 2024,
        string diploma = "diploma.pdf",
        EducationStatus status = EducationStatus.InProgress)
    {
        return new CvEducation
        {
            Id = id,
            PersonId = personId,
            MajorId = majorId,
            InstituteOrSchoolId = instituteId,
            StartYear = startYear,
            EndYear = endYear,
            Diploma = diploma,
            Status = status
        };
    }
}