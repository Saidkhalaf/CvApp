namespace cvapp.BL.Domain;

public class CronosEmployer
{
    public long Id { get; set; }
    public string FullName { get; set; }
    public string LogoUrl { get; set; }
    public string Jurisdiction { get; set; }
    public string CountryCode { get; set; }
    public string BtwNumber { get; set; }
    public string TopCluster { get; set; }
    public string Cluster { get; set; }
    public int Subnumber { get; set; }
    public string Status { get; set; }
    public string StatusReason { get; set; }
    public string OldWnr { get; set; }
    public ICollection<Career> Careers { get; set; } = new List<Career>();
    
    
    
}