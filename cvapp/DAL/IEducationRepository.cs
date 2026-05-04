using cvapp.BL.Domain;

namespace cvapp.DAL;

public interface IEducationRepository
{
    IEnumerable<Education> ReadAll();
    Education ReadById(long id);
    Education Create(Education education);
    IEnumerable<EducationOptions> ReadAllEducationOptions();
    EducationOptions ReadEducationOptionsById(long id);

}