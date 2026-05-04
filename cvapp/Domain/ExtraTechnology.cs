using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class ExtraTechnology
{
    public long Id { get; set; }
    [Required]
   
    public string Domain { get; set; }
    [Required]
    public TechnicalLevel Level { get; set; }
    public long LevelId { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }
    public Technology Technology { get; set; } 
    public long TechnologyId { get; set; }
}