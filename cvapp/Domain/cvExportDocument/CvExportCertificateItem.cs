namespace cvapp.BL.Domain.cvExportDocument;

public class CvExportCertificateItem
{
    public string Name { get; set; } = string.Empty;
    public string InstituteName { get; set; } = string.Empty;
    public string CertificateCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public DateTime ExpiresOn { get; set; }
}