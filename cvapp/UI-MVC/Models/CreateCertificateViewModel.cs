using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace CVAPP.UI.Web.MVC.Models;

public class CreateCertificateViewModel
{
    [Required(ErrorMessage = "Please enter a name for the certificate.")] 
    public string Name { get; set; } = string.Empty;

     [Required(ErrorMessage = "Type is verplicht.")]
    public CertificateType? Type { get; set; }
    
    public string ReturnUrl { get; set; } = string.Empty;
     public bool IsEmbedded { get; set; }
}