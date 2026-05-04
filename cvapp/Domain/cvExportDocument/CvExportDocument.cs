namespace cvapp.BL.Domain.cvExportDocument;

public class CvExportDocument
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public List<CvExportLanguageItem> Languages { get; set; } = new();
    public List<CvExportEducationItem> Educations { get; set; } = new();
    public List<CvExportCertificateItem> Certificates { get; set; } = new();
    public List<CvExportTrainingItem> Trainings { get; set; } = new();
    public byte[] LogoBytes { get; set; }
    public string LogoContentType { get; set; }
    public bool HasLogo => LogoBytes is { Length: > 0 };

    public string LogoDataUri
    {
        get
        {
            if (!HasLogo || string.IsNullOrWhiteSpace(LogoContentType))
            {
                return string.Empty;
            }

            return $"data:{LogoContentType};base64,{Convert.ToBase64String(LogoBytes!)}";
        }
    }
}







