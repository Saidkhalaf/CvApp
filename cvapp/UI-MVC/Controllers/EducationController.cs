using BL;
using CVAPP.UI.Web.MVC.Models;
using cvapp.ui.web.mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cvapp.ui.web.mvc.Controllers;

public class EducationController : Controller
{
    private readonly IEducationManager _educationManager;

    public EducationController(IEducationManager educationManager)
    {
        _educationManager = educationManager;
    }

    [HttpGet]
    public IActionResult Create(string returnUrl = null, bool embedded = false)
    {
        var model = new CreateEducationViewModel
        {
            EducationLevels = GetEducationLevelSelectList(),
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateEducationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.EducationLevels = GetEducationLevelSelectList();
            return View(model);
        }

        try
        {
           var createdEducation = _educationManager.AddEducation(
                model.Name,
                model.Eqf,
                model.IsQualified,
                model.EducationLevelId);

            TempData["SuccessMessage"] = "Opleiding succesvol toegevoegd.";
            TempData["MessageSection"] = "opleidingen";
            
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl.WithQueryValues(("selectedEducationId", createdEducation.Id.ToString())));
            }

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.EducationLevels = GetEducationLevelSelectList();
            return View(model);
        }
    }


    private List<SelectListItem> GetEducationLevelSelectList()
    {
        return _educationManager.GetAllEducationOptions()
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.EducationLevel.ToString()
            })
            .ToList();
    }
}
