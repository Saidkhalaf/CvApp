namespace CVAPP.UI.Web.MVC.Models;

public class CvEducationLookupCardViewModel
{
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string BadgeText { get; set; } = "LOOKUP";
    public string Placeholder { get; set; } = string.Empty;
    public string SelectedName { get; set; } = string.Empty;
    public string HelperText { get; set; } = string.Empty;
    public string CreateController { get; set; } = string.Empty;
    public string CreateAction { get; set; } = "Create";
    public string CreateReturnUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }
    public string CreateTitle { get; set; } = string.Empty;
    public long? Value { get; set; }
}
