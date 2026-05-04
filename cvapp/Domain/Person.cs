using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class Person : IValidatableObject
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Voornaam is verplicht.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Achternaam is verplicht.")]
    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }

    [Required(ErrorMessage = "Straat is verplicht.")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Huisnummer is verplicht.")]
    public string HouseNumber { get; set; }

    public string Bus { get; set; }

    [Required(ErrorMessage = "Postcode is verplicht.")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "Gemeente is verplicht.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Land is verplicht.")]
    public string Country { get; set; }

    [Required(ErrorMessage = "E-mail is verplicht.")]
    [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres op.")]
    public string Email { get; set; }

    public string Phone { get; set; }
    public string LinkedInUrl { get; set; }
    
    [Required(ErrorMessage = "Functie/Titel is verplicht.")]
    [StringLength(100)]
    [Display(Name = "Professionele Titel")]
    public string JobTitle { get; set; }
    
    [Required(ErrorMessage = "Persoonlijke samenvatting is verplicht.")]
    [StringLength(1000, ErrorMessage = "De samenvatting mag maximaal 1000 tekens bevatten.")]
    [Display(Name = "Over mij")]
    public string Summary { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public ICollection<ExtraExpertise.ExtraExpertise> ExtraExpertises { get; set; } = new List<ExtraExpertise.ExtraExpertise>();
    public ICollection<ExtraTechnology> ExtraTechnologies { get; set; } = new List<ExtraTechnology>();
    public ICollection<Career> Careers { get; set; } = new List<Career>();
    public ICollection<CvCertificate> Certificates { get; set; } = new List<CvCertificate>();
    public ICollection<CourseTraining> CoursesTrainings { get; set; } = new List<CourseTraining>();
    public ICollection<CvEducation> Educations { get; set; } = new List<CvEducation>();
    // todo does person have a project ? now project -- client 
    //public ICollection<CvProject> Projects { get; set; } = new List<CvProject>();
    public ICollection<Language> Languages { get; set; } = new List<Language>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth.HasValue && DateOfBirth.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Geboortedatum kan niet in de toekomst liggen.",
                [nameof(DateOfBirth)]);
        }

        if (!string.IsNullOrWhiteSpace(LinkedInUrl) && !Uri.IsWellFormedUriString(LinkedInUrl, UriKind.Absolute))
        {
            yield return new ValidationResult(
                "LinkedIn URL is ongeldig.",
                [nameof(LinkedInUrl)]);
        }
        
        if (!string.IsNullOrWhiteSpace(Summary) && Summary.Length < 10)
        {
            yield return new ValidationResult(
                "Je 'Over mij' sectie is wel erg kort. Vertel iets meer over jezelf!",
                [nameof(Summary)]);
        }
    }
}