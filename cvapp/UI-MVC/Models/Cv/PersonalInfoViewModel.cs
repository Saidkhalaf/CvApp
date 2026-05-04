using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;
using cvapp.BL.Domain;
using CVAPP.UI.Web.MVC.Models;
using CVAPP.UI.Web.MVC.Models.Training;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace cvapp.ui.web.mvc.Models.Cv;

public class PersonalInfoViewModel
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Voornaam is verplicht.")]
    [Display(Name = "Voornaam")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Achternaam is verplicht.")]
    [Display(Name = "Achternaam")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Geboortedatum")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Geslacht")]
    public Gender? Gender { get; set; }

    [Display(Name = "Burgerlijke staat")]
    public MaritalStatus? MaritalStatus { get; set; }

    [Required(ErrorMessage = "Straat is verplicht.")]
    [Display(Name = "Straat")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "Huisnummer is verplicht.")]
    [Display(Name = "Huisnummer")]
    public string HouseNumber { get; set; } = string.Empty;

    [Display(Name = "Bus")]
    public string Bus { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postcode is verplicht.")]
    [Display(Name = "Postcode")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gemeente is verplicht.")]
    [Display(Name = "Gemeente")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Land is verplicht.")]
    [Display(Name = "Land")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail is verplicht.")]
    [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres op.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefoon")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "LinkedIn URL")]
    public string LinkedInUrl { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Functie/Titel is verplicht.")]
    [StringLength(100)]
    [Display(Name = "Professionele Titel")]
    public string JobTitle { get; set; }
    
    [Required(ErrorMessage = "Persoonlijke samenvatting is verplicht.")]
    [StringLength(1000, ErrorMessage = "De samenvatting mag maximaal 1000 tekens bevatten.")]
    [Display(Name = "Over mij")]
    public string Summary { get; set; }

    public string Section { get; set; } = "persoon-gegevens";

    public List<CvEducation> CvEducations { get; set; } = new();

    // These nested models belong to other section forms/controllers and should not affect personal-info POST validation.
    [ValidateNever]
    public CreateCvEducationViewModel NewEducation { get; set; } = new();

    public List<Language> Languages { get; set; } = new();

    public List<EditTrainingViewModel> Trainings { get; set; } = new();
    
    [ValidateNever]
    public CreateLanguageViewmodel NewLanguage { get; set; } = new();
    
    public List<CvCertificate> CvCertificates { get; set; } = new();

    [ValidateNever]
    public CreateCvCertificateViewmodel NewCertificate { get; set; } = new();

}


