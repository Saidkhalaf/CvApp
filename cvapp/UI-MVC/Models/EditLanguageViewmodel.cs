using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.ui.web.mvc.Models;

public class EditLanguageViewmodel
{
    public long Id { get; set; }
     [Required]
    public LanguageName Name { get; set; }
    [Required]
    public LanguageLevel LanguageLevel { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }
}