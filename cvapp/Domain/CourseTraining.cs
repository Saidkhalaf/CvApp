using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;
public class CourseTraining : IValidatableObject
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Naam van de cursus/training is verplicht.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Naam moet minstens 2 letters zijn.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\.\,\-\:\(\)\/]+$", ErrorMessage = "Naam bevat ongeldige tekens.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Jaar is verplicht.")]
    [Range(1900, 2100, ErrorMessage = "Jaar moet tussen 1900 en 2100 liggen.")]
    public int Year { get; set; }

    public InstituteOrSchool InstituteOrSchool { get; set; }
    public long? InstituteOrSchoolId { get; set; }

    public CourseType CourseType { get; set; }

    
    public Exam Exam { get; set; }
    public long? ExamId { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    { 
        var results = new List<ValidationResult>();

        // ===== Year Validation =====
        if (Year < 1900 || Year > DateTime.Now.Year + 5)
        {
            results.Add(new ValidationResult(
                $"Jaar moet tussen 1900 en {DateTime.Now.Year + 5} liggen.",
                [nameof(Year)]));
        }

        // ONLY check Exam Result logic IF the exam object actually exists
        if (Exam != null)
        {
            if (Exam.Result == ExamResult.Certified && string.IsNullOrWhiteSpace(Exam.Name))
            {
                results.Add(new ValidationResult(
                    "Een gecertificeerd examen moet een naam hebben.",
                    ["Exam.Name"]));
            }
        }

        // ===== Person Relationship =====
        if (PersonId <= 0)
        {
            results.Add(new ValidationResult(
                "Een cursus/training moet gekoppeld zijn aan een persoon.",
                [nameof(PersonId)]));
        }

        return results;
    }
}