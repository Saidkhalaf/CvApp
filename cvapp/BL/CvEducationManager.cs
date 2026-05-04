using System;
using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL;

namespace BL;

public class CvEducationManager: ICvEducationManager
{
       private readonly ICvEducationRepository _repository;

    public CvEducationManager(ICvEducationRepository repository)
    {
        _repository = repository;
    }

    public CvEducation AddCvEducation(
        long personId,
        long educationId,
        long instituteOrSchoolId,
        int startYear,
        int endYear,
        string diploma,
        EducationStatus status)
    {

        var education = _repository.ReadEducationById(educationId);
        if (education == null)
            throw new ArgumentException("Invalid education id.");

        var institute = _repository.ReadInstituteOrSchoolById(instituteOrSchoolId);
        if (institute == null)
            throw new ArgumentException("Invalid institute or school id.");

        var cvEducation = new CvEducation(
             personId,
            educationId,
           instituteOrSchoolId,
           startYear,
            endYear,
             diploma,
             status
        );

        return _repository.CreateCvEducation(cvEducation);
    }

    public CvEducation GetCvEducationById(long id)
    {
        return _repository.ReadCvEducationById(id);
    }

    public IEnumerable<CvEducation> GetCvEducationsByPersonId(long personId)
    {
        return _repository.ReadCvEducationsByPersonId(personId);
    }

    public IEnumerable<Education> GetAllEducations()
    {
        return _repository.ReadAllEducations();
    }

    public IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools()
    {
        return _repository.ReadAllInstituteOrSchools();
    }

    public InstituteOrSchool AddInstituteOrSchool(string name, InstituteType? type, string email, string website,
        string telephoneNumber, string fax, string btwNumber, string comments)
    {

        var institute = new InstituteOrSchool
        (
            name,
            type,
            email,
            website,
            telephoneNumber,
            fax,
            btwNumber,
            comments
        );

        return _repository.CreateInstituteOrSchool(institute);
    }

    public void UpdateCvEducation(
        long id,
        long educationId,
        long instituteOrSchoolId,
        int startYear,
        int endYear,
        string diploma,
        EducationStatus status)
    {

        var cvEducation = _repository.ReadCvEducationById(id);
        if (cvEducation == null)
            throw new ArgumentException("CvEducation not found.");

        var education = _repository.ReadEducationById(educationId);
        if (education == null)
            throw new ArgumentException("Invalid education id.");

        var institute = _repository.ReadInstituteOrSchoolById(instituteOrSchoolId);
        if (institute == null)
            throw new ArgumentException("Invalid institute or school id.");

        cvEducation.MajorId = educationId;
        cvEducation.InstituteOrSchoolId = instituteOrSchoolId;
        cvEducation.StartYear = startYear;
        cvEducation.EndYear = endYear;
        cvEducation.Diploma = diploma;
        cvEducation.Status = status;

        _repository.UpdateCvEducation(cvEducation);
    }

    public void RemoveCvEducation(long id)
    {
        var cvEducation = _repository.ReadCvEducationById(id);
        if (cvEducation == null)
            throw new ArgumentException("CvEducation not found.");

        _repository.DeleteCvEducation(cvEducation);
    }
}
