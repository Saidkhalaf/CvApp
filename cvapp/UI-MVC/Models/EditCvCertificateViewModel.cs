using System.ComponentModel.DataAnnotations;

namespace CVAPP.UI.Web.MVC.Models;

public class EditCvCertificateViewModel
{
    public long Id { get; set; }
    [Required]
    [Range(1900,3000)]
    public int Year { get; set; }
    [Required]
    public long CertificateId { get; set; }
    public long InstituteOrSchoolId { get; set; }
    public string CertificateCode { get; set; }
    public IFormFile UploadedCertificate { get; set; }
    public string ExistingCertificate { get; set; }
    
    public DateTime ExpiresOn { get; set; }
    public string SelectedCertificateName { get; set; } = string.Empty;
    public string SelectedInstituteOrSchoolName { get; set; } = string.Empty;
    public string ReturnUrl { get; set; }
    public bool IsEmbedded { get; set; }
    public List<CertificateLookupRowViewModel> CertificateLookupRows { get; set; } = new();
    public List<InstituteLookupRowViewModel> InstituteLookupRows { get; set; } = new();

}