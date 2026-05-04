using BL;
using cvapp.BL.Domain;
using cvapp.ui.web.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace cvapp.ui.web.mvc.Controllers;

public class LanguageController(ILanguageManager manager) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(manager.GetAlLanguages());
    }

    [HttpGet]
    public IActionResult Add(string returnUrl = null, bool embedded = false)
    {
        return View(new CreateLanguageViewmodel
        {
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        });
    }

  

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(long languageId, string returnUrl = null)
    {
        try
        {
            manager.RemoveLanguage(languageId);
            TempData["SuccessMessage"] = "Taal verwijderd.";
            TempData["MessageSection"] = "talen";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            TempData["MessageSection"] = "talen";
        }

        return RedirectToReturnUrlOrIndex(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(CreateLanguageViewmodel clv, string returnUrl = null)
    {
        clv.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? clv.ReturnUrl : returnUrl;

        if (!ModelState.IsValid)
        {
            return View(clv);
        }

        try
        {
            manager.AddLanguage(clv.Name, clv.LanguageLevel);
            TempData["SuccessMessage"] = "Taal toegevoegd.";
            TempData["MessageSection"] = "talen";
            return RedirectToReturnUrlOrIndex(clv.ReturnUrl);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(clv);
        }
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddInline([Bind(Prefix = "NewLanguage")] CreateLanguageViewmodel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Controleer de taalgegevens en probeer opnieuw.";
            TempData["MessageSection"] = "talen";
            return RedirectToReturnUrlOrIndex(returnUrl);
        }

        try
        {
            manager.AddLanguage(model.Name, model.LanguageLevel);
            TempData["SuccessMessage"] = "Taal toegevoegd.";
            TempData["MessageSection"] = "talen";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            TempData["MessageSection"] = "talen";
        }

        return RedirectToReturnUrlOrIndex(returnUrl);
    }
    
    [HttpGet]
    public IActionResult Edit(long languageId, string returnUrl = null, bool embedded = false)
    {
        var language = manager.GetLanguageById(languageId);

        if (language == null)
        {
            return NotFound();
        }

        var vm = new EditLanguageViewmodel
        {
            Id = language.Id,
            Name = language.LanguageName,
            LanguageLevel = language.LanguageLevel,
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditLanguageViewmodel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            manager.EditLanguage(vm.Id, vm.Name, vm.LanguageLevel);
            TempData["SuccessMessage"] = "Taal bijgewerkt.";
            TempData["MessageSection"] = "talen";
            return RedirectToReturnUrlOrIndex(vm.ReturnUrl);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }
    
    private IActionResult RedirectToReturnUrlOrIndex(string returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(CvController.Create), "Cv", new { personId = AppDefaults.DefaultPersonId, section = "talen"});
    }
 }