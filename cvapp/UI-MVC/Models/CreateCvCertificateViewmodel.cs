using System.ComponentModel.DataAnnotations;

namespace CVAPP.UI.Web.MVC.Models;

public class CreateCvCertificateViewmodel
{
    [Required]
    [Range(1900,3000)]
    public int Year { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }
    [Required]
    public long CertificateId { get; set; }
    public long InstituteOrSchoolId { get; set; }
    public string CertificateCode { get; set; }
    public IFormFile UploadedCertificate { get; set; }
    public string ExistingCertificateName { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string SelectedCertificateName { get; set; } = string.Empty;
    public string SelectedInstituteOrSchoolName { get; set; } = string.Empty;
    public List<CertificateLookupRowViewModel> CertificateLookupRows { get; set; } = new();
    public List<InstituteLookupRowViewModel> InstituteLookupRows { get; set; } = new();

}