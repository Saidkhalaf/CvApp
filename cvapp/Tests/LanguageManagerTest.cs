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

public class LanguageManagerTests
{
    private readonly Mock<ILanguageRepository> _repoMock;
    private readonly LanguageManager _manager;

    public LanguageManagerTests()
    {
        _repoMock = new Mock<ILanguageRepository>();
        _manager = new LanguageManager(_repoMock.Object);
    }

    [Fact]
    public void GetAlLanguages_ShouldReturnAllLanguages()
    {
        var languages = new List<Language>
        {
            new Language(1, LanguageLevel.Beginner, LanguageName.Arabisch) { Id = 1 },
            new Language(1, LanguageLevel.Native, LanguageName.Nederlands) { Id = 2 }
        };

        _repoMock.Setup(r => r.ReadAllLanguages()).Returns(languages);

        var result = _manager.GetAlLanguages();

        result.Should().HaveCount(2);
        _repoMock.Verify(r => r.ReadAllLanguages(), Times.Once);
    }

    [Fact]
    public void GetLanguageById_ShouldReturnLanguage()
    {
        var language = new Language(1, LanguageLevel.Beginner, LanguageName.Arabisch) { Id = 1 };

        _repoMock.Setup(r => r.ReadLanguage(1)).Returns(language);

        var result = _manager.GetLanguageById(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.LanguageName.Should().Be(LanguageName.Arabisch);
    }

    [Fact]
    public void GetLanguagesByPersonId_ShouldReturnLanguagesOfThatPerson()
    {
        var languages = new List<Language>
        {
            new Language(1, LanguageLevel.Beginner, LanguageName.Arabisch) { Id = 1 },
            new Language(1, LanguageLevel.Intermediate, LanguageName.Engels) { Id = 2 }
        };

        _repoMock.Setup(r => r.ReadLanguagesOfPersonId(1)).Returns(languages);

        var result = _manager.GetLanguagesByPersonId(1);

        result.Should().HaveCount(2);
        _repoMock.Verify(r => r.ReadLanguagesOfPersonId(1), Times.Once);
    }

    [Fact]
    public void AddLanguage_ShouldCreateLanguageWithDefaultPersonId()
    {
        Language? savedLanguage = null;

        _repoMock
            .Setup(r => r.CreateLanguage(It.IsAny<Language>()))
            .Callback<Language>(l => savedLanguage = l);

        _repoMock
            .Setup(r => r.ExistsByPersonAndLanguageName(AppDefaults.DefaultPersonId, LanguageName.Arabisch))
            .Returns(false);

        var result = _manager.AddLanguage(LanguageName.Arabisch, LanguageLevel.Beginner);

        result.Should().NotBeNull();
        result.PersonId.Should().Be(AppDefaults.DefaultPersonId);
        result.LanguageName.Should().Be(LanguageName.Arabisch);
        result.LanguageLevel.Should().Be(LanguageLevel.Beginner);

        savedLanguage.Should().NotBeNull();
        savedLanguage!.PersonId.Should().Be(AppDefaults.DefaultPersonId);

        _repoMock.Verify(r => r.CreateLanguage(It.IsAny<Language>()), Times.Once);
    }

    [Fact]
    public void AddLanguage_ShouldThrowException_WhenLanguageAlreadyExistsForPerson()
    {
        _repoMock
            .Setup(r => r.ExistsByPersonAndLanguageName(AppDefaults.DefaultPersonId, LanguageName.Arabisch))
            .Returns(true);

        Action act = () => _manager.AddLanguage(LanguageName.Arabisch, LanguageLevel.Beginner);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("This language already exists for this person.");
    }

    [Fact]
    public void RemoveLanguage_ShouldDeleteLanguage_WhenLanguageExists()
    {
        var language = new Language(1, LanguageLevel.Beginner, LanguageName.Arabisch) { Id = 1 };

        _repoMock.Setup(r => r.ReadLanguage(1)).Returns(language);

        _manager.RemoveLanguage(1);

        _repoMock.Verify(r => r.ReadLanguage(1), Times.Once);
        _repoMock.Verify(r => r.DeleteLanguage(language), Times.Once);
    }

    [Fact]
    public void RemoveLanguage_ShouldThrowException_WhenLanguageNotFound()
    {
        _repoMock.Setup(r => r.ReadLanguage(99)).Returns((Language)null!);

        Action act = () => _manager.RemoveLanguage(99);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Language not found.");
    }

    [Fact]
    public void EditLanguage_ShouldUpdateLanguage_WhenNoDuplicateExists()
    {
        var existingLanguage = new Language(1, LanguageLevel.Beginner, LanguageName.Arabisch) { Id = 1 };

        _repoMock.Setup(r => r.ReadLanguage(1)).Returns(existingLanguage);
        _repoMock
            .Setup(r => r.ExistsAnotherByPersonIdAndLanguageName(1, AppDefaults.DefaultPersonId, LanguageName.Engels))
            .Returns(false);
        _repoMock.Setup(r => r.UpdateLanguage(existingLanguage)).Returns(existingLanguage);

        var result = _manager.EditLanguage(1, LanguageName.Engels, LanguageLevel.Intermediate);

        result.LanguageName.Should().Be(LanguageName.Engels);
        result.LanguageLevel.Should().Be(LanguageLevel.Intermediate);

        _repoMock.Verify(r => r.UpdateLanguage(existingLanguage), Times.Once);
    }

    [Fact]
    public void EditLanguage_ShouldThrowException_WhenLanguageNotFound()
    {
        _repoMock.Setup(r => r.ReadLanguage(99)).Returns((Language)null!);

        Action act = () => _manager.EditLanguage(99, LanguageName.Engels, LanguageLevel.Intermediate);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Language not found.");
    }

    [Fact]
    public void EditLanguage_ShouldThrowException_WhenLanguageAlreadyExistsForPerson()
    {
        var existingLanguage = new Language(AppDefaults.DefaultPersonId, LanguageLevel.Beginner, LanguageName.Arabisch)
            { Id = 1 };

        _repoMock.Setup(r => r.ReadLanguage(1)).Returns(existingLanguage);
        _repoMock
            .Setup(r => r.ExistsAnotherByPersonIdAndLanguageName(1, AppDefaults.DefaultPersonId, LanguageName.Engels))
            .Returns(true);

        Action act = () => _manager.EditLanguage(1, LanguageName.Engels, LanguageLevel.Intermediate);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("This language already exists for this person.");
    }

   
}
