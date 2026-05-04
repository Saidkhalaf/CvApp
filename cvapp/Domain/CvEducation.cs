using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class CvEducation
{
    public long Id { get; set; }

    public int StartYear { get; set; }
    public int EndYear { get; set; }

    [Required]
    public Education Major { get; set; } 
    public long MajorId { get; set; }
    /*
    public string Level { get; set; }
    public int? Eqf { get; set; }*/

    [Required]
    public InstituteOrSchool InstituteOrSchool { get; set; }
    public long InstituteOrSchoolId { get; set; }
    
    public string Diploma { get; set; }

    public EducationStatus Status { get; set; }

    public long PersonId { get; set; }
    public Person Person { get; set; }

   public CvEducation(long personId , long majorId  , long instituteOrSchoolId , int startYear , int endYear , string diploma,  EducationStatus status)
    {
        PersonId = personId;
        MajorId = majorId;
        InstituteOrSchoolId = instituteOrSchoolId;
        StartYear = startYear;
        EndYear = endYear;
        Diploma = diploma;
        Status = status;
    }

    public CvEducation()
    {
        
    }
}