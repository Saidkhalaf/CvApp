using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain.enums;

public enum MaritalStatus
{
    [Display(Name = "Ongehuwd")]
    Single,
    [Display(Name = "Gehuwd")]
    Married,
    [Display(Name = "Gescheiden")]
    Divorced,
    [Display(Name = "Weduwe/Weduwnaar")]
    Widowed,
    [Display(Name = "Samenwonend")]
    LivingTogether
}