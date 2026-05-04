using System.Collections.Specialized;
using System.Net.Quic;
using BL;
using cvapp.BL.Domain;
using CVAPP.UI.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers;

public class CvEducationController : Controller
{
    private const string EducationSection = "opleidingen";
    private readonly ICvEducationManager _cvEducationManager;

    public CvEducationController(ICvEducationManager cvEducationManager)
    {
        _cvEducationManager = cvEducationManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var educations = _cvEducationManager.GetCvEducationsByPersonId(AppDefaults.DefaultPersonId);

        ViewBag.PersonId = AppDefaults.DefaultPersonId;
        return View(educations);
    }

    [HttpGet]
    public IActionResult Create(long? selectedEducationId, long? selectedInstituteId, string returnUrl = null, bool embedded = false)
    {
        var model = new CreateCvEducationViewModel
        {
            PersonId = AppDefaults.DefaultPersonId,
            EducationId = selectedEducationId,
            InstituteOrSchoolId = selectedInstituteId,
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };

        PopulateSelections(model);
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateCvEducationViewModel model, string returnUrl = null)
    {
        model.PersonId = AppDefaults.DefaultPersonId;
        model.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? model.ReturnUrl : returnUrl;

        if (!ModelState.IsValid)
        {
            PopulateSelections(model);
            return View(model);
        }

        return CreateEducationAndRedirect(model, model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult Edit(long id, string returnUrl = null, bool embedded = false)
    {
        var cvEducation = _cvEducationManager.GetCvEducationById(id);
        if (cvEducation == null)
        {
            return NotFound();
        }

        var model = new EditCvEducationViewModel
        {
            Id = cvEducation.Id,
            PersonId = AppDefaults.DefaultPersonId,
            StartYear = cvEducation.StartYear,
            EndYear = cvEducation.EndYear,
            EducationId = cvEducation.MajorId,
            InstituteOrSchoolId = cvEducation.InstituteOrSchoolId,
            ExistingDiplomaFileName = cvEducation.Diploma,
            Status = cvEducation.Status,
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };

        PopulateSelections(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditCvEducationViewModel model)
    {
        model.PersonId = AppDefaults.DefaultPersonId;

        if (!ModelState.IsValid)
        {
            PopulateSelections(model);
            return View(model);
        }

        try
        {
            var diplomaFileName = SaveDiplomaFile(model.Diploma, model.ExistingDiplomaFileName);
            
            _cvEducationManager.UpdateCvEducation(
                model.Id,
                model.EducationId!.Value,
                model.InstituteOrSchoolId!.Value,
                model.StartYear!.Value,
                model.EndYear!.Value,
                diplomaFileName,
                model.Status);

            SetEducationSectionMessage(successMessage: "Opleiding bijgewerkt.");
            return CompleteCreateOrEdit(model.ReturnUrl, model.IsEmbedded);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            PopulateSelections(model);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(long id, string returnUrl)
    {
        try
        {
            _cvEducationManager.RemoveCvEducation(id);
            SetEducationSectionMessage(successMessage: "Opleiding verwijderd.");
        }
        catch (ArgumentException ex)
        {
            SetEducationSectionMessage(errorMessage: ex.Message);
        }

        return RedirectToReturnUrlOrIndex(returnUrl);
    }

    private IActionResult CreateEducationAndRedirect(CreateCvEducationViewModel model, string returnUrl)
    {
        var diplomaFileName = SaveDiplomaFile(model.Diploma);

        _cvEducationManager.AddCvEducation(
            model.PersonId,
            model.EducationId!.Value,
            model.InstituteOrSchoolId!.Value,
            model.StartYear,
            model.EndYear,
            diplomaFileName,
            model.Status);

        SetEducationSectionMessage(successMessage: "Opleiding toegevoegd.");
        return CompleteCreateOrEdit(returnUrl, model.IsEmbedded);
    }

    private IActionResult CompleteCreateOrEdit(string returnUrl, bool isEmbedded)
    {
        var cvCreateUrl = Url.Action("Create", "Cv", new { section = "opleidingen" });
    
        var targetUrl = ResolveLocalReturnUrlOrFallback(returnUrl, cvCreateUrl ?? "/Cv/Create");

        if (!isEmbedded)
        {
            return Redirect(targetUrl);
        }

        // This view handles closing the modal and redirecting the parent window
        return View("~/Views/Shared/EmbeddedRedirect.cshtml", targetUrl);
    }

    private IActionResult RedirectToReturnUrlOrIndex(string returnUrl)
    {
        var cvCreateUrl = Url.Action("Create", "Cv", new { section = "opleidingen" });
        return Redirect(ResolveLocalReturnUrlOrFallback(returnUrl, cvCreateUrl ?? "/Cv/Create"));
    }

    private static string SaveDiplomaFile(IFormFile diploma, string existingFileName = "")
    {
        if (diploma == null)
        {
            return existingFileName;
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diplomas");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(diploma.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            diploma.CopyTo(stream);
        }

        return fileName;
    }

    private void PopulateSelections(CreateCvEducationViewModel model)
    {
        var lookups = BuildLookupData(model.EducationId, model.InstituteOrSchoolId);
        model.EducationLookupRows = lookups.EducationRows;
        model.InstituteLookupRows = lookups.InstituteRows;
        model.SelectedEducationName = string.IsNullOrWhiteSpace(lookups.SelectedEducationName)
            ? model.SelectedEducationName
            : lookups.SelectedEducationName;
        model.SelectedInstituteOrSchoolName = string.IsNullOrWhiteSpace(lookups.SelectedInstituteName)
            ? model.SelectedInstituteOrSchoolName
            : lookups.SelectedInstituteName;
    }

    private void PopulateSelections(EditCvEducationViewModel model)
    {
        var lookups = BuildLookupData(model.EducationId, model.InstituteOrSchoolId);
        model.EducationLookupRows = lookups.EducationRows;
        model.InstituteLookupRows = lookups.InstituteRows;
        model.SelectedEducationName = string.IsNullOrWhiteSpace(lookups.SelectedEducationName)
            ? model.SelectedEducationName
            : lookups.SelectedEducationName;
        model.SelectedInstituteOrSchoolName = string.IsNullOrWhiteSpace(lookups.SelectedInstituteName)
            ? model.SelectedInstituteOrSchoolName
            : lookups.SelectedInstituteName; 
    }

  

    private (List<EducationLookupRowViewModel> EducationRows,
        List<InstituteLookupRowViewModel> InstituteRows,
        string SelectedEducationName,
        string SelectedInstituteName) BuildLookupData(long? selectedEducationId, long? selectedInstituteId)
    {
        var educations = _cvEducationManager.GetAllEducations().ToList();
        var institutes = _cvEducationManager.GetAllInstituteOrSchools().ToList();

        return (
            educations.Select(education => new EducationLookupRowViewModel
            {
                Id = education.Id,
                Name = education.Name,
                EducationLevel = education.EducationLevel?.EducationLevel.ToString() ?? string.Empty,
                EqfLevel = education.Eqf,
                IsQualified = education.IsQualified
            }).ToList(),
            institutes.Select(institute => new InstituteLookupRowViewModel
            {
                Id = institute.Id,
                Name = institute.Name,
                Type = institute.Type.ToString(),
                Email = institute.Email,
                Website = institute.Website,
                TelephoneNumber = institute.TelephoneNumber,
                BtwNumber = institute.BtwNumber, 
                // Selected = selectedInstituteId == i.Id
            }).ToList(),
            educations.FirstOrDefault(education => education.Id == selectedEducationId)?.Name ?? string.Empty,
            institutes.FirstOrDefault(institute => institute.Id == selectedInstituteId)?.Name ?? string.Empty
        );
    }
    [HttpGet]
    public IActionResult DownloadDiploma(long id, string returnUrl = null)
    {
        var cvEducation = _cvEducationManager.GetCvEducationById(id);
        if (cvEducation == null)
        {
             return RedirectToDiplomaError("Diploma entry not found.", returnUrl);
        }

        if (string.IsNullOrWhiteSpace(cvEducation.Diploma))
        {
            return RedirectToDiplomaError("No diploma uploaded.", returnUrl);

        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diplomas");
        var filePath = Path.Combine(uploadsFolder, cvEducation.Diploma);

        if (!System.IO.File.Exists(filePath))
        {
            return RedirectToDiplomaError("Diploma has not been uploaded!", returnUrl);

        }

        return PhysicalFile(filePath, "application/octet-stream", cvEducation.Diploma);
    }

    private IActionResult RedirectToDiplomaError(string message, string returnUrl)
    {
        SetEducationSectionMessage(errorMessage: message);
        return RedirectToReturnUrlOrEducationSection(returnUrl);
    }

    private IActionResult RedirectToReturnUrlOrEducationSection(string returnUrl)
    {
        var educationSectionUrl = Url.Action("Create", "Cv", new { section = "opleidingen" }) ?? "/Cv/Create?section=opleidingen";
        return Redirect(ResolveLocalReturnUrlOrFallback(returnUrl, $"{educationSectionUrl}#opleidingen"));
    }

    private void SetEducationSectionMessage(string successMessage = null, string errorMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(successMessage))
        {
            TempData["SuccessMessage"] = successMessage;
        }

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            TempData["ErrorMessage"] = errorMessage;
        }

        TempData["MessageSection"] = EducationSection;
    }

    private string ResolveLocalReturnUrlOrFallback(string returnUrl, string fallbackUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        return fallbackUrl;
        
    }

   

}
