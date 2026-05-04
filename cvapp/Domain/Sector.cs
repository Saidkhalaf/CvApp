using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain;

public class Sector
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } 
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Career > Careers { get; set; } = new List<Career>();
    public ICollection<ExtraExpertise.ExtraExpertise> ExtraExpertises { get; set; } = new List<ExtraExpertise.ExtraExpertise>();
}