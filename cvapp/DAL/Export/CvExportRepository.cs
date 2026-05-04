using cvapp.BL.Domain;
using cvapp.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace cvapp.DAL.Export;

public class CvExportRepository(CvAppDbContext context) : ICvExportRepository
{
    public Person ReadPersonById(long personId)
    {
        return context.Persons
            .AsNoTracking()
            .SingleOrDefault(person => person.Id == personId);
    }

    public IReadOnlyList<Language> ReadLanguagesByPersonId(long personId)
    {
        return context.Languages
            .AsNoTracking()
            .Where(language => language.PersonId == personId)
            .OrderBy(language => language.LanguageName)
            .ToList();
    }

    public IReadOnlyList<CvEducation> ReadEducationsByPersonId(long personId)
    {
        return context.CvEducations
            .AsNoTracking()
            .Include(education => education.Major)
            .ThenInclude(major => major.EducationLevel)
            .Include(education => education.InstituteOrSchool)
            .Where(education => education.PersonId == personId)
            .OrderByDescending(education => education.EndYear)
            .ThenByDescending(education => education.StartYear)
            .ToList();
    }

    public IReadOnlyList<CvCertificate> ReadCertificatesByPersonId(long personId)
    {
        return context.CvCertificates
            .AsNoTracking()
            .Include(certificate => certificate.CertificateName)
            .Include(certificate => certificate.InstituteOrSchool)
            .Where(certificate => certificate.PersonId == personId)
            .OrderByDescending(certificate => certificate.Year)
            .ToList();
    }

    public IReadOnlyList<CourseTraining> ReadTrainingsByPersonId(long personId)
    {
        return context.CoursesTrainings
            .AsNoTracking()
            .Include(training => training.InstituteOrSchool)
            .Where(training => training.PersonId == personId)
            .OrderByDescending(training => training.Year)
            .ToList();
    }
}
