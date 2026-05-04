using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CVAPP.UI.Web.MVC.Models.Training;

public class EditTrainingViewModel
{
    // Primary Key for the training record
    public long Id { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Jaar is verplicht")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Type cursus is verplicht")]
    public CourseType CourseType { get; set; }

    // Link to the person this CV belongs to
    public long PersonId { get; set; }
    public long? InstituteOrSchoolId { get; set; }
    public long? ExamId { get; set; }
    
    public CreateInstituteViewModel InstituteOrSchool { get; set; } = new();
    
    public CreateExamViewModel Exam { get; set; } = new();

    // SelectLists for the dropdowns in the UI
    public IEnumerable<SelectListItem> InstituteOrSchools { get; set; }
    public IEnumerable<SelectListItem> Exams { get; set; }
}