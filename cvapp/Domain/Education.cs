using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class Education
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int Eqf { get; set; }
    public bool IsQualified { get; set; }
    public DateTime CreatedAt { get; set; }
    public EducationOptions EducationLevel { get; set; }
    public long EducationLevelId { get; set; }
    public int Total { get; set; }
    public ICollection<CvEducation> CvEducations { get; set; } = new List<CvEducation>();
}