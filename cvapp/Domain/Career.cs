using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain;

public class Career
{
    public long Id { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public CronosEmployer CronosEmployer { get; set; }
    public long ChronosEmployerId { get; set; }
    public Employer Employer { get; set; }
    public long EmployerId { get; set; }
    
    [Required]
    public Sector Sector { get; set; }
    public long SectorId { get; set; }
    public string Description { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }  
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>(); 
    public long ProfileId { get; set; }
}

