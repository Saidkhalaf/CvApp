using BL.Export;
using cvapp.BL.Domain;
using cvapp.BL.Domain.cvExportDocument;
using CVAPP.UI.Web.MVC.Models.Export;
using CVAPP.UI.Web.MVC.Services.Export;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers.Export;

public class CvExportController : Controller
{
    private readonly ICvExportManager _cvExportManager;

    private readonly IPdfExportService _pdfExportService; 
    
    public CvExportController(ICvExportManager cvExportManager, IPdfExportService pdfExportService)
    {
        _cvExportManager = cvExportManager;
        _pdfExportService = pdfExportService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View(new CvExportViewModel());
    }
   
   
    [HttpPost]
    public IActionResult Preview(CvExportViewModel model)
    {
        if (!TryBuildDocumentFromViewModel(model, out var document) || document is null)
        {
            return BadRequest("De preview kon niet opgebouwd worden.");
        }

        var pdfBytes = _pdfExportService.GeneratePdf(document, model.TemplateName);

        Response.Headers.ContentDisposition = "inline; filename=\"cv-preview.pdf\"";

        return File(pdfBytes, "application/pdf");
    }
    
    
  
    [HttpPost]
    public IActionResult ExportToPdf(CvExportViewModel model)
    {
        if (!TryBuildDocumentFromViewModel(model, out var document))
        {
            return View("Index", model);
        }

        var pdfBytes = _pdfExportService.GeneratePdf(document, model.TemplateName);

        var fileName = $"cv-{document.FullName.Replace(' ', '-')}-{DateTime.Now:yyyyMMdd-HHmm}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
    
    private bool TryBuildDocumentFromViewModel(CvExportViewModel model, out CvExportDocument document)
    {
        document = null;

        if (!ValidateLogo(model))
        {
            return false;
        }

        var options = new CvExportOptions
        {
            PersonId = AppDefaults.DefaultPersonId,
            IncludeCertificates = model.IncludeCertificates,
            IncludeEducation = model.IncludeEducations,
            IncludeTrainings = model.IncludeTrainings,
            IncludeLanguages = model.IncludeLanguages,
            TemplateName = model.TemplateName
        };
        document = _cvExportManager.BuildDocument(options);
        if (model.LogoFile is { Length: > 0 })
        {
            using var memoryStream = new MemoryStream();
            model.LogoFile.CopyTo(memoryStream);

            var logoBytes = memoryStream.ToArray();

            HttpContext.Session.Set("CvExportLogo", logoBytes);
            HttpContext.Session.SetString("CvExportLogoContentType", model.LogoFile.ContentType);

            document.LogoBytes = logoBytes;
            document.LogoContentType = model.LogoFile.ContentType;
            return true;
        }

        var savedLogoBytes = HttpContext.Session.Get("CvExportLogo");
        var savedContentType = HttpContext.Session.GetString("CvExportLogoContentType");

        if (savedLogoBytes is { Length: > 0 } && !string.IsNullOrWhiteSpace(savedContentType))
        {
            document.LogoBytes = savedLogoBytes;
            document.LogoContentType = savedContentType;
        }

        return true;
    }

    private bool ValidateLogo(CvExportViewModel model)
    {
        if (model.LogoFile is null || model.LogoFile.Length == 0)
        {
            return true;
        }

        
        const long maxLogoSizeBytes = 2 * 1024 * 1024;
        var allowedContentTypes = new[] { "image/png", "image/jpeg" };
        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
        var extension = Path.GetExtension(model.LogoFile.FileName).ToLowerInvariant();

        if (model.LogoFile.Length > maxLogoSizeBytes ||
            !allowedContentTypes.Contains(model.LogoFile.ContentType) ||
            !allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError(nameof(model.LogoFile), "Upload een geldig PNG of JPG logo van maximaal 2 MB.");
            return false;
        }

        return true;
    }
}
