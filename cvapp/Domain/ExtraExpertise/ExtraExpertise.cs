using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain.ExtraExpertise;

public class ExtraExpertise
{
    public long Id { get; set; }

    [Required]
    public Expertise Expertise { get; set; } 
    public long ExpertiseId { get; set; }
  
    [Required]
    public TechnicalLevel Level { get; set; }
    public Methodology Methodology { get; set; }
  
    [Required]
    public Sector Sector { get; set; }
    public long SectorId { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }
}