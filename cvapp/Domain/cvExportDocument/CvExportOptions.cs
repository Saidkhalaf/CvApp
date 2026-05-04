namespace cvapp.BL.Domain.cvExportDocument;

public class CvExportOptions
{
    public long PersonId { get; set; }
    public bool IncludeLanguages { get; set; }
    public bool IncludeEducation { get; set; }
    public bool IncludeCertificates { get; set; }
    public bool IncludeTrainings { get; set; }
    public string TemplateName { get; set; } = "Standard";
}
