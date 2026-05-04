#nullable enable
using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CVAPP.UI.Web.MVC.Models.Training;

public class CreateTrainingViewModel
{
    public long PersonId { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }

    [Required(ErrorMessage = "Naam van de cursus/training is verplicht.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Naam moet tussen 2 en 200 tekens zijn.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\.\,\-\:\(\)\/]+$", ErrorMessage = "Naam bevat ongeldige tekens.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jaar is verplicht.")]
    [Range(1900, 2100, ErrorMessage = "Jaar moet tussen 1900 en 2100 liggen.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Selecteer een type cursus.")]
    public CourseType CourseType { get; set; }
    
    // --- NESTED DATA ---
    public CreateInstituteViewModel? InstituteOrSchool { get; set; } = new();
    public CreateExamViewModel? Exam { get; set; } = new();
    
    // Dropdowns for existing data (if the user selects instead of types)
    public long? InstituteOrSchoolId { get; set; }
    public long? ExamId { get; set; }

    public List<SelectListItem> CourseTypes { get; set; } = new();
    public List<SelectListItem> InstituteOrSchools { get; set; } = new();
    public List<SelectListItem> Exams { get; set; } = new();
}