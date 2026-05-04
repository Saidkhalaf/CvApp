namespace cvapp.BL.Domain.cvExportDocument;


public class CvExportEducationItem
{
    public string Name { get; set; } = string.Empty;
    public string InstituteName { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Eqf { get; set; }
    public int StartYear { get; set; }
    public int EndYear { get; set; }
}