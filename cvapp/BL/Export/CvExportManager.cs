using cvapp.BL.Domain.cvExportDocument;
using cvapp.DAL.Export;

namespace BL.Export;

public class CvExportManager : ICvExportManager
{
    private readonly ICvExportRepository _repository;

    public CvExportManager(ICvExportRepository repository)
    {
        _repository = repository;
    }

    public CvExportDocument BuildDocument(CvExportOptions options)
    {
        var person = _repository.ReadPersonById(options.PersonId);
        if (person == null)
        {
            throw new InvalidOperationException("Person not found.");
        }

        var document = new CvExportDocument
        {
            FullName = person.FullName,
            Email = person.Email ?? string.Empty,
            Phone = person.Phone ?? string.Empty,
            LinkedInUrl = person.LinkedInUrl ?? string.Empty,
            Address = BuildAddress(person.Street, person.HouseNumber, person.Bus, person.PostalCode, person.City, person.Country)
        };

        if (options.IncludeLanguages)
        {
            document.Languages = _repository.ReadLanguagesByPersonId(options.PersonId)
                .Select(l => new CvExportLanguageItem
                {
                    Name = l.LanguageName.ToString(),
                    Level = l.LanguageLevel.ToString()
                })
                .ToList();
        }

        if (options.IncludeEducation)
        {
            document.Educations = _repository.ReadEducationsByPersonId(options.PersonId)
                .Select(e => new CvExportEducationItem
                {
                    Name = e.Major.Name,
                    InstituteName = e.InstituteOrSchool.Name,
                    EducationLevel = e.Major.EducationLevel.EducationLevel.ToString(),
                    Eqf = e.Major.Eqf,
                    Status = e.Status.ToString(),
                    StartYear = e.StartYear,
                    EndYear = e.EndYear
                })
                .ToList();
        }

        if (options.IncludeCertificates)
        {
            document.Certificates = _repository.ReadCertificatesByPersonId(options.PersonId)
                .Select(c => new CvExportCertificateItem
                {
                    Name = c.CertificateName.Name,
                    InstituteName = c.InstituteOrSchool.Name,
                    CertificateCode = c.CertificateCode,
                    Year = c.Year,
                    ExpiresOn = c.ExpiresOn
                })
                .ToList();
        }
        
        if (options.IncludeTrainings)
        {
            document.Trainings = _repository.ReadTrainingsByPersonId(options.PersonId)
                .Select(c => new CvExportTrainingItem
                {
                    Name = c.Name,
                    InstituteName = c.InstituteOrSchool != null ? c.InstituteOrSchool.Name : string.Empty,
                    CourseType = c.CourseType.ToString(),
                    Year = c.Year
                })
                .ToList();
        }

        return document;
    }

    private static string BuildAddress(params string[] parts)
    {
        return string.Join(", ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
