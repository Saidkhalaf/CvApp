using BL.PersonalInfo;
using BL.Training;
using BL;
using cvapp.BL.Domain;
using CVAPP.UI.Web.MVC.Models;
using cvapp.ui.web.mvc.Models.Cv;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers;

public class CvController(
    IPersonalInfoManager personalInfoManager,
    ICvEducationManager cvEducationManager,
    ILanguageManager languageManager,
    ITrainingManager trainingManager,
   ICvCertificateManager cvCertificateManager) : Controller
{
    [HttpGet]
    public IActionResult Create(long? personId)
    {
        // Determine which ID to use
        var effectiveId = personId ?? AppDefaults.DefaultPersonId;
        
        var person = personalInfoManager.GetPersonWithTrainings(effectiveId);
        if (person == null) return NotFound();

        var model = MapToViewModel(person, "persoon-gegevens");
        
        // Use the helper to populate the Trainings list and other dependencies
        PopulateCreatePageData(model, person.Id);
        
        return View(model);
    }

    // Keep the original Create for manual submissions or "Next" buttons
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PersonalInfoViewModel model, string returnTo = "persoon-gegevens")
    {
        if (!ModelState.IsValid)
        {
            PopulateCreatePageData(model, model.Id);
            return View(model);
        }

        try
        {
            var savedPerson = SaveToManager(model);

            TempData["SuccessMessage"] = "Gegevens succesvol opgeslagen.";
            var redirectUrl = Url.Action(nameof(Create), new { personId = savedPerson.Id, section = returnTo });
            return Redirect($"{redirectUrl}#{returnTo}");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            PopulateCreatePageData(model, model.Id);
            return View(model);
        }
    }

    /**
     * NEW: Optimized AutoSave endpoint for AJAX/TypeScript
     */
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AutoSave(PersonalInfoViewModel model)
    {
        // For auto-save, we might be more lenient with validation, 
        // but let's check if the basic identity fields are there.
        if (!ModelState.IsValid) 
        {
            return Json(new { success = false, message = "Validation failed" });
        }

        try
        {
            var savedPerson = SaveToManager(model);
            
            // Return the ID in case this was a first-time save (Id was 0)
            // This ensures the TypeScript can update the hidden Id field.
            return Json(new { 
                success = true, 
                personId = savedPerson.Id,
                message = "Saved automatically" 
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /**
     * Helper to avoid code duplication between Create and AutoSave
     */
    private Person SaveToManager(PersonalInfoViewModel model)
    {
        var effectivePersonId = model.Id > 0 ? model.Id : AppDefaults.DefaultPersonId;
        
        return personalInfoManager.Add(
            effectivePersonId,
            model.FirstName,
            model.LastName,
            model.DateOfBirth,
            model.Gender,
            model.MaritalStatus,
            model.Street,
            model.HouseNumber,
            model.Bus,
            model.PostalCode,
            model.City,
            model.Country,
            model.Email,
            model.Phone,
            model.LinkedInUrl,
            model.JobTitle,
            model.Summary);
    }

    // private PersonalInfoViewModel BuildCreateViewModel(long? personId, string section, long? selectedEducationId, long? selectedInstituteId)
    // {
    //     var model = new PersonalInfoViewModel { Section = string.IsNullOrWhiteSpace(section) ? "persoon-gegevens" : section };
    //
    //     if (personId is > 0)
    //     {
    //         var person = personalInfoManager.GetById(personId.Value);
    //         if (person != null)
    //         {
    //             model = MapToViewModel(person, model.Section);
    //         }
    //     }
    //
    //     PopulateCreatePageData(model, personId, selectedEducationId, selectedInstituteId);
    //     return model;
    // }

    private void PopulateCreatePageData(PersonalInfoViewModel model, long? personId, long? selectedEducationId = null, long? selectedInstituteId = null)
    {
        var effectivePersonId = personId.GetValueOrDefault() > 0 ? personId!.Value : AppDefaults.DefaultPersonId;

        model.CvEducations = cvEducationManager.GetCvEducationsByPersonId(effectivePersonId).ToList();
        model.NewEducation.PersonId = effectivePersonId;
        model.NewEducation.EducationId = selectedEducationId ?? model.NewEducation.EducationId;
        model.NewEducation.InstituteOrSchoolId = selectedInstituteId ?? model.NewEducation.InstituteOrSchoolId;
        model.Languages = languageManager.GetLanguagesByPersonId(effectivePersonId).ToList();
        model.CvCertificates = cvCertificateManager.GetCvCertificatesByPersonId(effectivePersonId).ToList();
        model.Trainings = trainingManager.GetAllTrainings()
            .Where(t => t.PersonId == effectivePersonId)
            .Select(MapToEditTrainingViewModel)
            .ToList();
        model.Id = model.Id > 0 ? model.Id : effectivePersonId;
    }
    
    // Helper method to keep your controller DRY
    private static EditTrainingViewModel MapToEditTrainingViewModel(CourseTraining t)
    {
        return new EditTrainingViewModel
        {
            Id = t.Id,
            Name = t.Name,
            Year = t.Year,
            CourseType = t.CourseType,
            PersonId = t.PersonId,
            InstituteOrSchool = t.InstituteOrSchool != null ? new CreateInstituteViewModel 
            {
                Name = t.InstituteOrSchool.Name,
                Type = t.InstituteOrSchool.Type,
                Email = t.InstituteOrSchool.Email,
                Website = t.InstituteOrSchool.Website,
                TelephoneNumber = t.InstituteOrSchool.TelephoneNumber,
                BtwNumber = t.InstituteOrSchool.BtwNumber,
                Comments = t.InstituteOrSchool.Comments
            } : new CreateInstituteViewModel(),
            Exam = t.Exam != null ? new CreateExamViewModel
            {
                Name = t.Exam.Name,
                Result = t.Exam.Result,
            } : new CreateExamViewModel()
        };
    }

    private static PersonalInfoViewModel MapToViewModel(Person person, string section)
    {
        return new PersonalInfoViewModel
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            MaritalStatus = person.MaritalStatus,
            Street = person.Street,
            HouseNumber = person.HouseNumber,
            Bus = person.Bus,
            PostalCode = person.PostalCode,
            City = person.City,
            Country = person.Country,
            Email = person.Email,
            Phone = person.Phone,
            LinkedInUrl = person.LinkedInUrl,
            JobTitle = person.JobTitle,
            Summary = person.Summary,  
            Section = section
        };
    }
}