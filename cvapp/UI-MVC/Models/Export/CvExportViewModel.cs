namespace CVAPP.UI.Web.MVC.Models.Export;

public class CvExportViewModel
{
    public bool IncludeLanguages { get; set; } = true;
    public bool IncludeEducations { get; set; } = true;
    public bool IncludeCertificates { get; set; } = true;
    public bool IncludeTrainings { get; set; } = true;
    public string TemplateName { get; set; } = "Standard";
    public IFormFile LogoFile { get; set; } 
    public List<string> AvailableTemplates { get; set; } = new()
    {
        "Standard",
        "Professional",
        "Most used"
    };
}
