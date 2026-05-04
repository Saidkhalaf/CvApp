#nullable enable
using System.ComponentModel.DataAnnotations;
using BL.Training;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.ui.web.mvc.Controllers.Training;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.UnitTests.Training;

public class TrainingControllerCreateExamTests
{
    private readonly TrainingController  _controller;
    private readonly Mock<ITrainingManager> _managerMock;

    public TrainingControllerCreateExamTests()
    {
        _managerMock = new Mock<ITrainingManager>();
        _controller = new TrainingController(_managerMock.Object);
    }
    
    [Fact]
    public void CreateExam_Post_ValidModel_RedirectsToCreate()
    {
        // Arrange
        var model = new CreateExamViewModel
        {
            Name = "Test Exam",
            Result = ExamResult.Passed,
            SelectedInstituteId = 1,
            ReturnUrl = "/return/url",
            IsEmbedded = true
        };
        var createdExam = new Exam { Id = 1 };
        _managerMock.Setup(m => m.AddExam(model.Name, model.Result ?? ExamResult.NetApplicable)).Returns(createdExam);

        // Act
        var result = _controller.CreateExam(model) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Create", result.ActionName);
        Assert.Equal(model.SelectedInstituteId, result.RouteValues?["selectedInstituteId"]);
        Assert.Equal(createdExam.Id, result.RouteValues?["selectedExamId"]);
        Assert.Equal(model.ReturnUrl, result.RouteValues?["returnUrl"]);
        Assert.Equal(model.IsEmbedded, result.RouteValues?["embedded"]);
    }

    [Fact]
    public void CreateExam_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new CreateExamViewModel
        {
            Name = "", // Invalid
            Result = ExamResult.Passed
        };
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = _controller.CreateExam(model) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var resultModel = result.Model as CreateExamViewModel;
        Assert.NotNull(resultModel);
        Assert.NotNull(resultModel.ExamResults);
        _managerMock.Verify(m => m.AddExam(It.IsAny<string>(), It.IsAny<ExamResult>()), Times.Never);
    }

    [Fact]
    public void CreateExam_Post_ManagerThrowsValidationException_ReturnsViewWithModelError()
    {
        // Arrange
        var model = new CreateExamViewModel
        {
            Name = "Test Exam",
            Result = ExamResult.Passed
        };
        var exceptionMessage = "Validation error";
        _managerMock.Setup(m => m.AddExam(model.Name, model.Result ?? ExamResult.NetApplicable)).Throws(new ValidationException(exceptionMessage));

        // Act
        var result = _controller.CreateExam(model) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var resultModel = result.Model as CreateExamViewModel;
        Assert.NotNull(resultModel);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal(exceptionMessage, _controller.ModelState[string.Empty]?.Errors[0].ErrorMessage);
    }
}

