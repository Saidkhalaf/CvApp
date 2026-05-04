using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class EducationOptions
{
    public long Id { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public int EqfLevel { get; set; }
    public ICollection<Education> Educations { get; set; } = new List<Education>();
}