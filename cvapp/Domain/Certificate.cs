using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class Certificate
{
    public long Id { get; set; }
    public string Name { get; set; } 
    public CertificateType Type { get; set; }
    public ICollection<CvCertificate> CvCertificates { get; set; } = new List<CvCertificate>();
    
}