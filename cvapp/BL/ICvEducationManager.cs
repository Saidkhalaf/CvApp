using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace BL;

public interface ICvEducationManager
{
    CvEducation AddCvEducation(
        long personId,
        long educationId,
        long instituteOrSchoolId,
        int startYear,
        int endYear,
        string diploma,
        EducationStatus status);

    CvEducation GetCvEducationById(long id);
    IEnumerable<CvEducation> GetCvEducationsByPersonId(long personId);

    IEnumerable<Education> GetAllEducations();
    
    
    IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools();

    InstituteOrSchool AddInstituteOrSchool(
        string name,
        InstituteType? type,
        string email,
        string website,
        string telephoneNumber,
        string fax,
        string btwNumber,
        string comments);
    

    void UpdateCvEducation(
        long id,
        long educationId,
        long instituteOrSchoolId,
        int startYear,
        int endYear,
        string diploma,
        EducationStatus status);

    void RemoveCvEducation(long id);
    
}
