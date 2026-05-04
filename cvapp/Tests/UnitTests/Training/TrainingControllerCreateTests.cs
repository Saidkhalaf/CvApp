#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BL.Training;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.ui.web.mvc.Controllers.Training;
using CVAPP.UI.Web.MVC.Models;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Tests.UnitTests.Training;

public class TrainingControllerCreateTests
{
    private readonly TrainingController  _controller;
    private readonly Mock<ITrainingManager> _managerMock;

    public TrainingControllerCreateTests()
    {
        _managerMock = new Mock<ITrainingManager>();
        _controller = new TrainingController(_managerMock.Object);
    }
    
    [Fact]
    public void Create_Post_InvalidModelState_ReturnsViewWithModel()
    {
        //Arrange
        var model = new CreateTrainingViewModel();
        _controller.ModelState.AddModelError("Name", "Required");
        
        //Act 
        var result = _controller.Create(model) as PartialViewResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal("Create", result.ViewName);
        Assert.Equal(model, result.Model);
        
        //Verify the manager was NEVER called because the model was invalid 
        _managerMock.Verify(m => m.AddTraining(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CourseType>(),  It.IsAny<long>(), It.IsAny<string>(), It.IsAny<InstituteType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ExamResult>()), Times.Never);
    }

    [Fact]
    public void Create_Post_ValidModel_ReturnsJsonSuccess()
    {
        //Arrange
        var model = new CreateTrainingViewModel
        {
            Name = "Unit Test Training",
            Year = 2024,
            CourseType = CourseType.Online,
            PersonId = 1,
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };

        var createdTraining = new CourseTraining { Id = 123 };
        _managerMock.Setup(m => m.AddTraining(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CourseType>(),
                It.IsAny<long>(), It.IsAny<string>(), It.IsAny<InstituteType?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ExamResult?>()))
            .Returns(createdTraining);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
        
        //Act 
        var result = _controller.Create(model) as JsonResult;
        
        //Assert
        Assert.NotNull(result);
        
        // Extract data from anonymous json response
        var jsonValue = result.Value;
        if (jsonValue != null)
        {
            var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);
            var idProp = jsonValue.GetType().GetProperty("id")?.GetValue(jsonValue);

            Assert.Equal(true, successProp);
            Assert.Equal(123L, idProp);
        }

        //Verify the manager was actually called with the correct data 
        _managerMock.Verify(m => m.AddTraining(
            model.Name,
            model.Year,
            It.IsAny<CourseType>(), 
            It.IsAny<long>(),
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
            ),
            Times.Once
            );
    }

    [Fact]
    public void Create_Post_ManagerThrowsException_ReturnsJsonError()
    {
        // Arrange: Create a valid model but mock the manager to simulate a business logic failure
        var model = new CreateTrainingViewModel 
        { 
            Name = "Valid but triggers logic error",
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };
        _managerMock.Setup(m => m.AddTraining(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CourseType>(),
                It.IsAny<long>(), It.IsAny<string?>(), It.IsAny<InstituteType?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<ExamResult?>()))
            .Throws(new ValidationException("Database logic failed"));

        // Act: Attempt to save the training
        var result = _controller.Create(model) as JsonResult;

        // Assert: Verify the controller catches the exception and returns JSON with success = false
        Assert.NotNull(result);
        var jsonValue = result.Value;
        if (jsonValue == null) return;
        var successProp = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue);
        var messageProp = jsonValue.GetType().GetProperty("message")?.GetValue(jsonValue);

        Assert.Equal(false, successProp);
        Assert.Equal("Database logic failed", messageProp);
    }

    [Fact]
    public void Create_Get_PopulatesSelectLists()
    {
        // Arrange
        _managerMock.Setup(m => m.GetAllInstituteOrSchools()).Returns(new List<InstituteOrSchool>());
        _managerMock.Setup(m => m.GetAllExams()).Returns(new List<Exam>());

        // Act
        var result = _controller.Create() as ViewResult;
        var model = result?.Model as CreateTrainingViewModel;

        // Assert
        Assert.NotNull(model);
        Assert.NotNull(model.CourseTypes);
        if (model.InstituteOrSchool != null) Assert.NotNull(model.InstituteOrSchool.InstituteTypes);
        Assert.NotNull(model.Exams);
        if (model.Exam != null) Assert.NotNull(model.Exam.ExamResults);
    }
}

