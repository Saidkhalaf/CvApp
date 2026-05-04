using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.ui.web.mvc.Models;

public class CreateLanguageViewmodel
{
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }

    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid language.")]
    public LanguageName Name { get; set; }
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid language level.")]
    public LanguageLevel LanguageLevel { get; set; }
}