namespace cvapp.BL.Domain;

public class Technology
{
    public long Id { get; set; }
    public string Name { get; set; } 
    public Domain Domain { get; set; }
    public long DomainId { get; set; }
    public ICollection<ExtraTechnology> ExtraTechnologies { get; set; } = new List<ExtraTechnology>();
    
}