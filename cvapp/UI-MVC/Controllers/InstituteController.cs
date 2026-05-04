using BL;
using CVAPP.UI.Web.MVC.Models;
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cvapp.ui.web.mvc.Controllers;

public class InstituteController(ICvEducationManager cvEducationManager) : Controller
{
    [HttpGet]
    public IActionResult Create(string returnUrl = null, bool embedded = false)
    {
        var model = new CreateInstituteViewModel
        {
            InstituteTypes = GetInstituteTypeSelectList(),
            ReturnUrl = returnUrl ?? string.Empty,
            IsEmbedded = embedded
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateInstituteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.InstituteTypes = GetInstituteTypeSelectList();
            return View(model);
        }

        try
        {
            var createdInstitute = cvEducationManager.AddInstituteOrSchool(
                model.Name,
                model.Type,
                model.Email,
                model.Website,
                model.TelephoneNumber,
                model.Fax,
                model.BtwNumber,
                model.Comments);

            TempData["SuccessMessage"] = "Instituut/school succesvol toegevoegd.";
            
            
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                // 1. Determine which Controller we came from
                bool isFromOpleidingen = model.ReturnUrl.Contains("CvEducation", StringComparison.OrdinalIgnoreCase);
                string targetController = isFromOpleidingen ? "CvEducation" : "Training";
            
                // 2. Set the correct UI tab to open in the CV overview
                TempData["MessageSection"] = isFromOpleidingen ? "opleidingen" : "cursussen-trainingen";

                // 3. Redirect back to the form we came from
                return RedirectToAction("Create", targetController, new
                {
                    selectedInstituteId = createdInstitute.Id,
                    returnUrl = model.ReturnUrl, // This keeps the ORIGINAL return path to the CV
                    embedded = model.IsEmbedded
                });
            }

            return RedirectToAction(nameof(CvController.Create), "Cv", new { section = "opleidingen" });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.InstituteTypes = GetInstituteTypeSelectList();
            return View(model);
        }
    }


    private static List<SelectListItem> GetInstituteTypeSelectList()
    {
        return Enum.GetValues<InstituteType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString()
            })
            .ToList();
    }
}