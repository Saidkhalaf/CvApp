#nullable enable
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class InstituteOrSchool
{
    public long Id { get; set; }
    public string Name { get; set; } 
    public InstituteType? Type { get; set; } 
    public string? Email { get; set; } 
    public string? Website { get; set; } 
    public string? TelephoneNumber { get; set; } 
    public string? Fax { get; set; } 
    public string? BtwNumber { get; set; }
    public string? Comments { get; set; }
    public ICollection<CvEducation> CvEducations { get; set; } = new List<CvEducation>();
    public ICollection<CvCertificate> CvCertificates { get; set; } = new List<CvCertificate>();
    public ICollection<CourseTraining> CourseTrainings { get; set; } = new List<CourseTraining>();

    public InstituteOrSchool( string name, InstituteType? type, string email, string website, string telephoneNumber, string fax, string btwNumber, string comments)
    {
        
        Name = name;
        Type = type;
        Email = email;
        Website = website;
        TelephoneNumber = telephoneNumber;
        Fax = fax;
        BtwNumber = btwNumber;
        Comments = comments;
    }

    public InstituteOrSchool()
    {
        
    }
}