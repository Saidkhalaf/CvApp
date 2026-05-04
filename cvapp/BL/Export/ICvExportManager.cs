using cvapp.BL.Domain.cvExportDocument;

namespace BL.Export;

public interface ICvExportManager
{
    CvExportDocument BuildDocument(CvExportOptions options);
}