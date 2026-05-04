using cvapp.BL.Domain.enums;

namespace CVAPP.UI.Web.MVC.Models;

public class CertificateLookupRowViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string .Empty;
    public string Type { get; set; } = string.Empty;
}