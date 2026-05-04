using System.ComponentModel.DataAnnotations;

namespace cvapp.BL.Domain;


public class CvProject
{
    public long Id { get; set; }
    
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    [Required]
    public string Description { get; set; }
    public string Responsibilities { get; set; }
    public string TimesheetCode { get; set; }
    
    public long ProfileId { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }

    public long CareerId { get; set; }
    public Career Career { get; set; }
    
    public Client Client { get; set; }
    public long ClientId { get; set; }
    
    
    public GroupCompany GroupCompany { get; set; }
    public long GroupCompanyId { get; set; }
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}