using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace CVAPP.UI.Web.MVC.Models;

public class CreateCvEducationViewModel
{
    public long PersonId { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }

    [Range(1900, 2100, ErrorMessage = "Start year must be between 1900 and 2100.")]
    public int StartYear { get; set; }

    [Range(1900, 2100, ErrorMessage = "End year must be between 1900 and 2100.")]
    public int EndYear { get; set; }

    [Required(ErrorMessage = "Please select an education.")]
    public long? EducationId { get; set; }

    [Required(ErrorMessage = "Please select an institute or school.")]
    public long? InstituteOrSchoolId { get; set; }

    public IFormFile Diploma { get; set; }

    [Required(ErrorMessage = "Please select a status.")]
    public EducationStatus Status { get; set; }

    public string SelectedEducationName { get; set; } = string.Empty;
    public string SelectedInstituteOrSchoolName { get; set; } = string.Empty;
    public List<EducationLookupRowViewModel> EducationLookupRows { get; set; } = new();
    public List<InstituteLookupRowViewModel> InstituteLookupRows { get; set; } = new();
}
