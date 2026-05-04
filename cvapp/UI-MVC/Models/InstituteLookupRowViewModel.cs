namespace CVAPP.UI.Web.MVC.Models;

public class InstituteLookupRowViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public string TelephoneNumber { get; set; } = string.Empty;
    public string BtwNumber { get; set; } = string.Empty;
}
