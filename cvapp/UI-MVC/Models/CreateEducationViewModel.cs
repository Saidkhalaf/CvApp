using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CVAPP.UI.Web.MVC.Models;

public class CreateEducationViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1,10)]
    public int Eqf { get; set; }

    public bool IsQualified { get; set; }

    [Required]
    [Range(1,long.MaxValue)]
    

    public long EducationLevelId { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }


    public List<SelectListItem> EducationLevels { get; set; } = new();

}