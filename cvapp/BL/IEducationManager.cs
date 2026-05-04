using System.Collections.Generic;
using cvapp.BL.Domain;

namespace BL;

public interface IEducationManager
{
    IEnumerable<Education> GetAllEducations();
    Education GetEducationById(long id);
    Education AddEducation(string name, int eqf, bool isQualified, long educationLevelId);

    IEnumerable<EducationOptions> GetAllEducationOptions();
}