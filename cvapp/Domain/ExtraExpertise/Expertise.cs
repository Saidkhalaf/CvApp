namespace cvapp.BL.Domain.ExtraExpertise;

public class Expertise
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ICollection<ExtraExpertise> ExtraExpertises { get; set; } = new List<ExtraExpertise>();
    
}