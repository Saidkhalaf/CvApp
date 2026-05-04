using cvapp.BL.Domain.enums;
using Client = cvapp.BL.Domain.Client;

namespace cvapp.BL.Domain;

public class Project
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Name { get; set; }
    public CronosEmployer CronosEmployer { get; set; }
    public long CronosEmployerId { get; set; }
    public string TimeSheet { get; set; }
    public long GroupCompanyId { get; set; }
    public string GroupCompanyLogo { get; set; }
    public Sector Sector { get; set; }
    public long SectorId { get; set; }
    public Career Career { get; set; }
    public long CareerId { get; set; }
    public CvProject CvProject { get; set; }
    public long CvProjectId { get; set; }
    public ClientDivision ClientDivision { get; set; }
    public long ClientDivisionId { get; set; } 
    public long ProfileId { get; set; } 
    
    
    
    
    
    
}