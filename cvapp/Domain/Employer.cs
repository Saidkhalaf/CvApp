namespace cvapp.BL.Domain;

public class Employer
{
    
    public long Id { get; set; }
    public string Name { get; set; }
    public string BtwNumber { get; set; }
    public string MainOffice { get; set; }
    public ICollection<Employer> Employers { get; set; } = new List<Employer>();
    
}