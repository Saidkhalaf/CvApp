using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class CvCertificate
{
    public long Id { get; set; }

    [Required]
    public int Year { get; set; }
    [Required]
    public Certificate CertificateName { get; set; }
    public long CertificateId { get; set; }
    public InstituteOrSchool InstituteOrSchool { get; set; }
    public long InstituteOrSchoolId { get; set; }
    public string CertificateCode { get; set; }

    public string Certificate { get; set; }

    public DateTime ExpiresOn { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    public CvCertificate(int year, long certificateId,   long instituteOrSchoolId, string certificateCode, string certificate, DateTime expiresOn, long personId)
    {
        Year = year;
        CertificateId = certificateId;
        InstituteOrSchoolId = instituteOrSchoolId;
        CertificateCode = certificateCode;
        Certificate = certificate;
        ExpiresOn = expiresOn;
        PersonId = personId;
    }
}