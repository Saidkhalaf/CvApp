namespace cvapp.BL.Domain;

public class ClientDivision
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>(); 
}