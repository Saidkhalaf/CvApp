using BL;
using CVAPP.UI.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers;

public class CertificateController : Controller
{
    private readonly ICvCertificateManager _manager; 
    
    public CertificateController(ICvCertificateManager manager)
    {
        _manager = manager;
    }
    
    [HttpGet]
    public IActionResult Create(string returnUrl = null , bool embedded = false)
    {
        return View( new CreateCertificateViewModel
        {
            ReturnUrl = returnUrl ?? Url.Action("Create", "Cv"),
            IsEmbedded = embedded
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateCertificateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var certificate = _manager.AddCertificate(model.Name, model.Type!.Value);
        var targetUrl = AddSelectedCertificateToReturnUrl(model.ReturnUrl, certificate.Id);

        if (model.IsEmbedded)
        {
            return View("~/Views/Shared/EmbeddedRedirect.cshtml", targetUrl);
        }

        return Redirect(targetUrl); 
    }
    private string AddSelectedCertificateToReturnUrl(string returnUrl, long selectedCertificateId)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            return Url.Action("Create", "CvCertificate", new { selectedCertificateId }) ?? "/CvCertificate/Create";
        }

        var separator = returnUrl.Contains('?') ? "&" : "?";
        return $"{returnUrl}{separator}selectedCertificateId={selectedCertificateId}";
    }
}