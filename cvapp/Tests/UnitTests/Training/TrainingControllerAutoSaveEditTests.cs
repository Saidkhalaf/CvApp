#nullable enable
using System;
using BL.Training;
using cvapp.BL.Domain.enums;
using cvapp.ui.web.mvc.Controllers.Training;
using CVAPP.UI.Web.MVC.Models;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.UnitTests.Training;

public class TrainingControllerAutoSaveEditTests
{
    private readonly TrainingController  _controller;
    private readonly Mock<ITrainingManager> _managerMock;

    public TrainingControllerAutoSaveEditTests()
    {
        _managerMock = new Mock<ITrainingManager>();
        _controller = new TrainingController(_managerMock.Object);
    }
    
    [Fact]
    public void AutoSaveEdit_Post_ValidModel_ReturnsJsonSuccess()
    {
        // Arrange
        var model = new EditTrainingViewModel
        {
            Id = 1,
            Name = "Updated Training",
            Year = 2024,
            CourseType = CourseType.Online,
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };
        
        // Act
        var result = _controller.AutoSaveEdit(model) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);

        Assert.True((bool)(successProp ?? throw new InvalidOperationException()));
        _managerMock.Verify(m => m.UpdateTraining(
            model.Id,
            model.Name,
            model.Year,
            model.CourseType,
            model.InstituteOrSchoolId,
            model.ExamId,
            model.InstituteOrSchool.Name,
            model.InstituteOrSchool.Type,
            model.InstituteOrSchool.Email,
            model.InstituteOrSchool.Website,
            model.InstituteOrSchool.TelephoneNumber,
            model.InstituteOrSchool.Fax,
            model.InstituteOrSchool.BtwNumber,
            model.InstituteOrSchool.Comments,
            model.Exam.Name,
            model.Exam.Result
            ), Times.Once);
    }

    [Fact]
    public void AutoSaveEdit_Post_InvalidModel_ReturnsJsonError()
    {
        // Arrange
        var model = new EditTrainingViewModel
        {
            Id = 1,
            Name = "", // Invalid
            Year = 2024,
            CourseType = CourseType.Online,
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = _controller.AutoSaveEdit(model) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);
        var messageProp = jsonValue.GetType().GetProperty("message")?.GetValue(jsonValue);

        Assert.False((bool)(successProp ?? throw new InvalidOperationException()));
        Assert.Equal("Validation failed", messageProp);
        _managerMock.Verify(m => m.UpdateTraining(
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CourseType>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string>(),
            It.IsAny<InstituteType?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ExamResult?>()
            ), Times.Never);
    }

    [Fact]
    public void AutoSaveEdit_Post_ManagerThrowsException_ReturnsJsonError()
    {
        // Arrange
        var model = new EditTrainingViewModel
        {
            Id = 1,
            Name = "Updated Training",
            Year = 2024,
            CourseType = CourseType.Online,
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };
        var exceptionMessage = "Database error";
        _managerMock.Setup(m => m.UpdateTraining(
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CourseType>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string>(),
            It.IsAny<InstituteType?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ExamResult?>()
            )).Throws(new Exception(exceptionMessage));

        // Act
        var result = _controller.AutoSaveEdit(model) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);
        var messageProp = jsonValue.GetType().GetProperty("message")?.GetValue(jsonValue);

        Assert.False((bool)(successProp ?? throw new InvalidOperationException()));
        Assert.Equal(exceptionMessage, messageProp);
    }
}

