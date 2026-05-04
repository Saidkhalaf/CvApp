using BL;
using cvapp.BL.Domain;
using CVAPP.UI.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers;

public class CvCertificateController : Controller
{
    private readonly ICvCertificateManager _manager;
    
    private const string CvCertificateSection = "certificaten";
    public CvCertificateController(ICvCertificateManager manager)
    {
        _manager = manager;

    }
    public IActionResult Index()
    {
       var cvCertificates= _manager.GetAllCvCertificates();
        return View(cvCertificates);
    }

 

    [HttpGet]
    public IActionResult Create( long selectedCertificateId , long selectedInstituteId, string returnUrl = null , bool embedded= false)
    {
        var model = new CreateCvCertificateViewmodel
        {
            CertificateId = selectedCertificateId,
            InstituteOrSchoolId = selectedInstituteId,
            ReturnUrl = returnUrl,
            ExpiresOn = DateTime.Today.AddYears(1),
            IsEmbedded = embedded
        };
        PopulateSelections(model);
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateCvCertificateViewmodel createCvCertificateViewmodel , string returnUrl=null)
    {
        createCvCertificateViewmodel.ReturnUrl =
            string.IsNullOrWhiteSpace(returnUrl) ? createCvCertificateViewmodel.ReturnUrl : returnUrl;
        if (!ModelState.IsValid)
        {
            PopulateSelections(createCvCertificateViewmodel);
            return View(createCvCertificateViewmodel);
        }

        return CreateCertificateAndRedirect(createCvCertificateViewmodel, createCvCertificateViewmodel.ReturnUrl);


    }
    [HttpGet]
    public IActionResult Edit(long id , string returnUrl = null, bool embedded = false)
    {
        

        var cvCertificates = _manager.GetCvCertificateById(id);
        if (cvCertificates == null)
        {
            return NoContent();
        }
        var model = new EditCvCertificateViewModel()
        {
            Id= cvCertificates.Id,
            InstituteOrSchoolId = cvCertificates.InstituteOrSchoolId,
            CertificateId = cvCertificates.CertificateId,
            CertificateCode = cvCertificates.CertificateCode,
            Year = cvCertificates.Year,
            ExistingCertificate = cvCertificates.Certificate,
            ExpiresOn = cvCertificates.ExpiresOn,
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };
        PopulateSelections(model);
        return View(model);
    }

    public IActionResult CreateCertificateAndRedirect(CreateCvCertificateViewmodel createCvCertificateViewmodel , string returnUrl)
    {
        var savedCertificateFile = SaveCertificate(createCvCertificateViewmodel.UploadedCertificate); 
        
        _manager.AddCvCertificate(createCvCertificateViewmodel.Year, createCvCertificateViewmodel.CertificateId,
            createCvCertificateViewmodel.InstituteOrSchoolId, 
            createCvCertificateViewmodel.CertificateCode, savedCertificateFile,createCvCertificateViewmodel.ExpiresOn,
            AppDefaults.DefaultPersonId);
        
        SetCvCertificateSectionMessage(successMessage:"Certificaat is toegevoegd.");
        return CompleteCreateOrEdit(returnUrl, createCvCertificateViewmodel.IsEmbedded);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditCvCertificateViewModel editCvCertificateViewModel)
    {
        if (!ModelState.IsValid)
        {
         PopulateSelections(editCvCertificateViewModel);
         return View(editCvCertificateViewModel);
        }

        try
        {
            var savedCertificateFile = SaveCertificate(editCvCertificateViewModel.UploadedCertificate, editCvCertificateViewModel.ExistingCertificate);
            _manager.UpdateCvCertificate(editCvCertificateViewModel.Id, editCvCertificateViewModel.Year, editCvCertificateViewModel.CertificateId,
                editCvCertificateViewModel.InstituteOrSchoolId, editCvCertificateViewModel.CertificateCode, savedCertificateFile, editCvCertificateViewModel.ExpiresOn,
                AppDefaults.DefaultPersonId);
            SetCvCertificateSectionMessage(successMessage: "Certificaat is bijgewerkt.");
            return CompleteCreateOrEdit(editCvCertificateViewModel.ReturnUrl, editCvCertificateViewModel.IsEmbedded);

        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            PopulateSelections(editCvCertificateViewModel);
            return View(editCvCertificateViewModel);
        }
    }

    private IActionResult CompleteCreateOrEdit(string returnUrl, bool isEmbedded)
    {
        var targetUrl = ResolveLocalReturnUrlOrFallback(returnUrl,GetCertificateSectionUrl());
        if (!isEmbedded)
        {
            return Redirect(targetUrl);
        }

        return View("~/Views/Shared/EmbeddedRedirect.cshtml", targetUrl);
    }
    
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(long id , string returnUrl = null)
    {
        try
        {
            _manager.DeleteCvCertificate(id);
            SetCvCertificateSectionMessage(successMessage: "Certificaat is verwijderd.");
        }
        catch (Exception ex)
        {
            SetCvCertificateSectionMessage(errorMessage: $"Fout bij verwijderen certificaat: {ex.Message}");
        }

        return RedirectToReturnUrlOrCertificateSection(returnUrl); 
    }
    
    
    
    /// <summary>
    ///             
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    private IActionResult RedirectToReturnUrlOrCertificateSection(string returnUrl)
    {
        return Redirect(ResolveLocalReturnUrlOrFallback(returnUrl, GetCertificateSectionUrl()));
    }

    private string GetCertificateSectionUrl()
    {
        var baseUrl = Url.Action("Create", "Cv", new { section = CvCertificateSection })
                      ?? "/Cv/Create?section=certificaten";

        return $"{baseUrl}#{CvCertificateSection}";
    }

    private IActionResult RedirectToReturnUrlOrEducationSection(string returnUrl)
    {
        var certificateSectionUrl = Url.Action("Create", "Cv", new { section = "certificaten" }) ?? "/Cv/Create?section=certificaten";
        return Redirect(ResolveLocalReturnUrlOrFallback(returnUrl, $"{certificateSectionUrl}#certificaten"));
    }

    private void SetCvCertificateSectionMessage(string successMessage = null, string errorMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(successMessage))
        {
            TempData["SuccessMessage"] = successMessage;
        }

        if (!String.IsNullOrWhiteSpace(errorMessage))
        {
            TempData["ErrorMessage"] = errorMessage; 
        }

        TempData["MessageSection"] = CvCertificateSection;
    }
    private IActionResult RedirectToCvCertificateError(string message, string returnUrl)
    {
        SetCvCertificateSectionMessage(errorMessage: message);
        return RedirectToReturnUrlOrEducationSection(returnUrl);
    }
    private string ResolveLocalReturnUrlOrFallback(string returnUrl, string fallbackUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        return fallbackUrl;
        
    }
    private IActionResult RedirectToReturnUrlOrIndex(string returnUrl)
    {
        return Redirect(ResolveLocalReturnUrlOrFallback(returnUrl, Url.Action(nameof(Index)) ?? "/CvCertificate"));
    }


   
    private void PopulateSelections(CreateCvCertificateViewmodel model)
    {
        var lookups = BuildLookUpData(model.CertificateId, model.InstituteOrSchoolId);
        model.CertificateLookupRows = lookups.certificateRows;
        model.InstituteLookupRows = lookups.InstituteRows;
        model.SelectedCertificateName = lookups.selectedCertificateName;
        model.SelectedInstituteOrSchoolName = lookups.SelectedInstituteName;
    }

    private void PopulateSelections(EditCvCertificateViewModel model)
    {
        var lookups = BuildLookUpData(model.CertificateId, model.InstituteOrSchoolId);
        model.CertificateLookupRows = lookups.certificateRows;
        model.InstituteLookupRows = lookups.InstituteRows;
        model.SelectedCertificateName = lookups.selectedCertificateName;
        model.SelectedInstituteOrSchoolName = lookups.SelectedInstituteName;
    }


    private (List<CertificateLookupRowViewModel> certificateRows, List<InstituteLookupRowViewModel> InstituteRows,
        string selectedCertificateName,
        string SelectedInstituteName) BuildLookUpData(long? selectedCertificateId, long? selectedInstituteId)
    {
        var certificatesList = _manager.GetAllCertificates().ToList();
        var institutesList = _manager.GetAllInstituteOrSchools().ToList();
        return (
            certificatesList.Select(certificate => new CertificateLookupRowViewModel()
            {
                Id = certificate.Id,
                Name = certificate.Name,
                Type = certificate.Type.ToString()
                
            }).ToList(),
            institutesList.Select(institute => new InstituteLookupRowViewModel
            {
                Id = institute.Id,
                Name = institute.Name,
                Type = institute.Type.ToString(),
                Email = institute.Email,
                Website = institute.Website,
                TelephoneNumber = institute.TelephoneNumber,
                BtwNumber = institute.BtwNumber, 
            }).ToList(),
            certificatesList.FirstOrDefault(certificate => certificate.Id == selectedCertificateId)?.Name ?? string.Empty,
            institutesList.FirstOrDefault(institute => institute.Id == selectedInstituteId)?.Name ?? string.Empty
        );
        
    }    
    
    // certificate download and upload 
    public static string SaveCertificate(IFormFile certificate, string existingFileName = "")
    {
        if (certificate == null)
        {
            return existingFileName;
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diplomas");
        Directory.CreateDirectory(uploadsFolder);

        var filename = $"{Guid.NewGuid()}_{Path.GetFileName(certificate.FileName)}";
        var filePath = Path.Combine(uploadsFolder, filename);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            certificate.CopyTo(stream);
        }

        return filename;
    }

    [HttpGet]
    public IActionResult DownloadCertificate(long id, string returnUrl = null)
    {
        var cvCertificate = _manager.GetCvCertificateById(id);
        if (cvCertificate == null)
        {
            return RedirectToCvCertificateError("Diploma entry not found.", returnUrl);
        }

        if (string.IsNullOrWhiteSpace(cvCertificate.Certificate))
        {
            return RedirectToCvCertificateError("No diploma uploaded.", returnUrl);

        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diplomas");
        var filePath = Path.Combine(uploadsFolder, cvCertificate.Certificate);

        if (!System.IO.File.Exists(filePath))
        {
            return RedirectToCvCertificateError("Diploma has not been uploaded!", returnUrl);

        }

        return PhysicalFile(filePath, "application/octet-stream", cvCertificate.Certificate);
    }
    
}
