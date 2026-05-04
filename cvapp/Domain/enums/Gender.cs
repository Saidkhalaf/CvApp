using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain.enums;

public enum Gender
{
    [Display(Name = "Man")]
    Male,
    [Display(Name = "Vrouw")]
    Female,
    [Display(Name = "Anders")]
    Other
}