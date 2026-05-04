using BL.Training;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CVAPP.UI.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cvapp.ui.web.mvc.Controllers.Training;

public class TrainingController(ITrainingManager trainingManager) : Controller
{
    [HttpGet]
    public IActionResult Create(long? selectedInstituteId = null, long? selectedExamId = null, string returnUrl = null, bool embedded = false)
    {
        var model = new CreateTrainingViewModel
        {
            PersonId = AppDefaults.DefaultPersonId,
            InstituteOrSchoolId = selectedInstituteId,
            ExamId = selectedExamId,
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded,
            InstituteOrSchool = new CreateInstituteViewModel(),
            Exam = new CreateExamViewModel()
        };
        model.CourseTypes = GetCourseTypeSelectList(model.CourseType);
        model.InstituteOrSchool.InstituteTypes = GetInstituteTypeSelectList();
        model.Exams = GetExamList(model.ExamId);
        model.Exam.ExamResults = GetExamResultSelectList();
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public IActionResult Create(CreateTrainingViewModel model)
    {
        if (model.InstituteOrSchool == null || string.IsNullOrWhiteSpace(model.InstituteOrSchool.Name))
        {
            ModelState.Remove("InstituteOrSchool.Name");
            ModelState.Remove("InstituteOrSchool.Type");
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("InstituteOrSchool")).ToList())
            {
                ModelState[key]?.Errors.Clear();
                ModelState[key]?.ValidationState = ModelValidationState.Valid;
            }
        }

        if (model.Exam == null || string.IsNullOrWhiteSpace(model.Exam.Name))
        {
            ModelState.Remove("Exam.Name");
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Exam")).ToList())
            {
                ModelState[key]?.Errors.Clear();
                ModelState[key]?.ValidationState = ModelValidationState.Valid;
            }
        }

        if (!ModelState.IsValid)
        {
            model.CourseTypes = GetCourseTypeSelectList(model.CourseType);
            model.InstituteOrSchool?.InstituteTypes = GetInstituteTypeSelectList(model.InstituteOrSchool.Type);
            model.Exam?.ExamResults = GetExamResultSelectList(model.Exam.Result);
            return PartialView("Create", model);
        }

        try
        {
            var createdTraining = trainingManager.AddTraining(
                model.Name, model.Year, model.CourseType, model.PersonId,
                model.InstituteOrSchool?.Name, model.InstituteOrSchool?.Type,
                model.InstituteOrSchool?.Email, model.InstituteOrSchool?.Website,
                model.InstituteOrSchool?.TelephoneNumber, model.InstituteOrSchool?.Fax, model.InstituteOrSchool?.BtwNumber,
                model.InstituteOrSchool?.Comments, model.Exam?.Name, model.Exam?.Result);
            return Json(new { success = true, id = createdTraining.Id});
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AutoSaveEdit(EditTrainingViewModel model)
    {
        ModelState.Remove("InstituteOrSchool.ReturnUrl");
        ModelState.Remove("Exam.ReturnUrl");

        if (model.InstituteOrSchool == null || string.IsNullOrWhiteSpace(model.InstituteOrSchool.Name))
        {
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("InstituteOrSchool")).ToList())
            {
                ModelState[key]?.Errors.Clear();
                ModelState[key]?.ValidationState = ModelValidationState.Valid;
            }
        }

        if (model.Exam == null || string.IsNullOrWhiteSpace(model.Exam.Name))
        {
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Exam")).ToList())
            {
                ModelState[key]?.Errors.Clear();
                ModelState[key]?.ValidationState = ModelValidationState.Valid;
            }
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { key = x.Key, error = x.Value.Errors.First().ErrorMessage });
            return Json(new { success = false, message = "Validation failed", errors });
        }

        try
        {
            trainingManager.UpdateTraining(
                model.Id,
                model.Name,
                model.Year,
                model.CourseType,
                model.InstituteOrSchoolId,
                model.ExamId,
                model.InstituteOrSchool?.Name,
                model.InstituteOrSchool?.Type,
                model.InstituteOrSchool?.Email,
                model.InstituteOrSchool?.Website,
                model.InstituteOrSchool?.TelephoneNumber,
                model.InstituteOrSchool?.Fax,
                model.InstituteOrSchool?.BtwNumber,
                model.InstituteOrSchool?.Comments,
                model.Exam?.Name,
                model.Exam?.Result
            );

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Edit(long id)
    {
        var training = trainingManager.GetTrainingById(id);
        if (training == null) return NotFound();

        var model = new EditTrainingViewModel
        {
            Id = training.Id,
            PersonId = training.PersonId,
            Name = training.Name,
            Year = training.Year,
            CourseType = training.CourseType,
            InstituteOrSchoolId = training.InstituteOrSchoolId,
            ExamId = training.ExamId,

            InstituteOrSchool = new CreateInstituteViewModel
            {
                Name = training.InstituteOrSchool?.Name,
                Type = training.InstituteOrSchool?.Type,
                Email = training.InstituteOrSchool?.Email,
                Website = training.InstituteOrSchool?.Website,
                TelephoneNumber = training.InstituteOrSchool?.TelephoneNumber,
                BtwNumber = training.InstituteOrSchool?.BtwNumber,
                Comments = training.InstituteOrSchool?.Comments,
                InstituteTypes = GetInstituteTypeSelectList(training.InstituteOrSchool?.Type)
            },

            Exam = new CreateExamViewModel
            {
                Name = training.Exam?.Name,
                Result = training.Exam?.Result,
                ExamResults = GetExamResultSelectList(training.Exam?.Result)
            }
        };

        return PartialView("Edit", model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(long id)
    {
        try
        {
            trainingManager.DeleteTraining(id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult CreateExam(CreateExamViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ExamResults = GetExamResultSelectList(model.Result);
            return View(model);
        }

        try
        {
            var createdExam = trainingManager.AddExam(model.Name, model.Result ?? ExamResult.NetApplicable);
            return RedirectToAction(nameof(Create), new
            {
                selectedInstituteId = model.SelectedInstituteId,
                selectedExamId = createdExam.Id,
                returnUrl = model.ReturnUrl,
                embedded = model.IsEmbedded
            });
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.ExamResults = GetExamResultSelectList(model.Result);
            return View(model);
        }
    }

    private List<SelectListItem> GetCourseTypeSelectList(CourseType? selectedCourseType = null)
    {
        return Enum.GetValues<CourseType>()
            .Where(courseType => courseType != CourseType.Unknown)
            .Select(courseType => new SelectListItem
            {
                Value = ((int)courseType).ToString(),
                Text = courseType.ToString(),
                Selected = selectedCourseType == courseType
            }).ToList();
    }
    
    private List<SelectListItem> GetExamList(long? selectedExamId = null)
    {
        return trainingManager.GetAllExams()
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = selectedExamId == t.Id
            }).ToList();
    }

    private static List<SelectListItem> GetExamResultSelectList(ExamResult? selectedResult = null)
    {
        return Enum.GetValues<ExamResult>()
            .Select(result => new SelectListItem
            {
                Value = ((int)result).ToString(),
                Text = result.ToString(),
                Selected = selectedResult == result
            }).ToList();
    }
    private List<SelectListItem> GetInstituteTypeSelectList(InstituteType? selectedType = null)
    {
        return Enum.GetValues<InstituteType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString(),
                Selected = selectedType == t
            }).ToList();
    }
}