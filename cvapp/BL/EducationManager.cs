using System;
using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.DAL;

namespace BL;

public class EducationManager : IEducationManager
{
    private readonly IEducationRepository _repository;

    public EducationManager(IEducationRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Education> GetAllEducations()
    {
        return _repository.ReadAll();
    }

    public Education GetEducationById(long id)
    {
        return _repository.ReadById(id);
    }

    public Education AddEducation(string name, int eqf, bool isQualified, long educationLevelId)
    {

        var education = new Education
        {
            Name = name.Trim(),
            Eqf = eqf,
            IsQualified = isQualified,
            EducationLevelId = educationLevelId,
            CreatedAt = DateTime.UtcNow,
            Total = 0
        };

        return _repository.Create(education);
    }

    public IEnumerable<EducationOptions> GetAllEducationOptions()
    {
        return _repository.ReadAllEducationOptions();
    }
}