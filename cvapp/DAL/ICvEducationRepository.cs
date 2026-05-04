using cvapp.BL.Domain;

namespace cvapp.DAL;

public interface ICvEducationRepository
{
    CvEducation CreateCvEducation(CvEducation cvEducation);
    CvEducation ReadCvEducationById(long id);
    IEnumerable<CvEducation> ReadCvEducationsByPersonId(long personId);

    IEnumerable<Education> ReadAllEducations();
    Education ReadEducationById(long id);

    IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools();
    InstituteOrSchool ReadInstituteOrSchoolById(long id);
    InstituteOrSchool CreateInstituteOrSchool(InstituteOrSchool instituteOrSchool);

    bool InstituteOrSchoolNameExists(string name);

    void UpdateCvEducation(CvEducation cvEducation);
    void DeleteCvEducation(CvEducation cvEducation);
}
