namespace cvapp.BL.Domain;

public class Client
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Photo { get; set; }
    public string BtwNumber { get; set; }
    public string MainOffice { get; set; }
    public string Logo { get; set; }
    public ICollection<ExtraTechnology> ExtraTechnologies { get; set; } = new List<ExtraTechnology>();
}