namespace cvapp.BL.Domain;

public class Domain
{
    public long Id { get; set; }
    public string Name { get; set; } 
    public string Description { get; set; }
    public ICollection<Technology> Technologies { get; set; } = new List<Technology>();
}