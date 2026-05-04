using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain.enums;

public enum LanguageLevel
{
    [Display(Name = "Basis")]
    Beginner = 1,

    [Display(Name = "Gemiddeld")]
    Intermediate = 2,

    [Display(Name = "Uitstekend")]
    Excellent = 3,

    [Display(Name = "Moedertaal")]
    Native = 4
}