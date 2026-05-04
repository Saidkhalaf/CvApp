namespace cvapp.BL.Domain;

public class GroupCompany
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string VatNumber { get; set; }
    public string Logo { get; set; }
    public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
}