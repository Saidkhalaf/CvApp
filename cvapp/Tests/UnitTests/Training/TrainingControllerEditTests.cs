#nullable enable
using System.Collections.Generic;
using BL.Training;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.ui.web.mvc.Controllers.Training;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.UnitTests.Training;

public class TrainingControllerEditTests
{
    private readonly TrainingController  _controller;
    private readonly Mock<ITrainingManager> _managerMock;

    public TrainingControllerEditTests()
    {
        _managerMock = new Mock<ITrainingManager>();
        _controller = new TrainingController(_managerMock.Object);
    }
    
    [Fact]
    public void Edit_Get_TrainingFound_ReturnsPartialViewWithModel()
    {
        // Arrange
        const long trainingId = 1;
        var training = new CourseTraining
        {
            Id = trainingId,
            Name = "Test Training",
            Year = 2024,
            CourseType = CourseType.Online,
            PersonId = 1,
            InstituteOrSchool = new InstituteOrSchool { Name = "Test School" },
            Exam = new Exam { Name = "Test Exam" }
        };
        _managerMock.Setup(m => m.GetTrainingById(trainingId)).Returns(training);
        _managerMock.Setup(m => m.GetAllExams()).Returns(new List<Exam>());

        // Act
        var result = _controller.Edit(trainingId) as PartialViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Edit", result.ViewName);
        var model = result.Model as EditTrainingViewModel;
        Assert.NotNull(model);
        Assert.Equal(training.Id, model.Id);
        Assert.Equal(training.Name, model.Name);
    }

    [Fact]
    public void Edit_Get_TrainingNotFound_ReturnsNotFound()
    {
        // Arrange
        const long trainingId = 1;
        _managerMock.Setup(m => m.GetTrainingById(trainingId)).Returns((CourseTraining)null!);

        // Act
        var result = _controller.Edit(trainingId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

