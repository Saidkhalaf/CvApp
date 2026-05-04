using cvapp.BL.Domain;

namespace cvapp.DAL.Export;

public interface ICvExportRepository
{
    Person ReadPersonById(long personId);
    IReadOnlyList<Language> ReadLanguagesByPersonId(long personId);
    IReadOnlyList<CvEducation> ReadEducationsByPersonId(long personId);
    IReadOnlyList<CvCertificate> ReadCertificatesByPersonId(long personId);
    IReadOnlyList<CourseTraining> ReadTrainingsByPersonId(long personId);
}
