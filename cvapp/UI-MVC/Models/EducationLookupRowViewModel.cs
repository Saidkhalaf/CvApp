namespace CVAPP.UI.Web.MVC.Models;

public class EducationLookupRowViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public int EqfLevel { get; set; }
    public bool IsQualified { get; set; }
}
