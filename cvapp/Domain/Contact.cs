using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain;

public class Contact
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } 
    [Required]
    public string ProfRelation { get; set; } 
    
    public string Details { get; set; } 

    public long CareerId { get; set; }
    public Career Career { get; set; }
}