#nullable enable
using System;
using BL.Training;
using cvapp.ui.web.mvc.Controllers.Training;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.UnitTests.Training;

public class TrainingControllerDeleteTests
{
    private readonly TrainingController  _controller;
    private readonly Mock<ITrainingManager> _managerMock;

    public TrainingControllerDeleteTests()
    {
        _managerMock = new Mock<ITrainingManager>();
        _controller = new TrainingController(_managerMock.Object);
    }
    
    [Fact]
    public void Delete_Post_Successful_ReturnsJsonSuccess()
    {
        // Arrange
        const long trainingId = 1;
        _managerMock.Setup(m => m.DeleteTraining(trainingId));

        // Act
        var result = _controller.Delete(trainingId) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);

        Assert.True((bool)(successProp ?? throw new InvalidOperationException()));
        _managerMock.Verify(m => m.DeleteTraining(trainingId), Times.Once);
    }

    [Fact]
    public void Delete_Post_ManagerThrowsException_ReturnsJsonError()
    {
        // Arrange
        const long trainingId = 1;
        var exceptionMessage = "Database error";
        _managerMock.Setup(m => m.DeleteTraining(trainingId)).Throws(new Exception(exceptionMessage));

        // Act
        var result = _controller.Delete(trainingId) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);
        var messageProp = jsonValue.GetType().GetProperty("message")?.GetValue(jsonValue);

        Assert.False((bool)(successProp ?? throw new InvalidOperationException()));
        Assert.Equal(exceptionMessage, messageProp);
        _managerMock.Verify(m => m.DeleteTraining(trainingId), Times.Once);
    }
}

