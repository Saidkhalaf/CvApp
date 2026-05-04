using cvapp.BL.Domain.cvExportDocument;

namespace CVAPP.UI.Web.MVC.Services.Export;

public interface IPdfExportService
{
    byte[] GeneratePdf(CvExportDocument document, string templateName);
}