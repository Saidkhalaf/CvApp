using cvapp.BL.Domain;

namespace cvapp.DAL;

public interface ICvCertificateRepository
{
    CvCertificate CreateCvCertificate(CvCertificate cvCertificate);

    CvCertificate UpdateCvCertificate(CvCertificate cvCertificate);

    CvCertificate ReadCvCertificateById(long id);
    Certificate ReadCertificateById(long certificateId);
    IEnumerable<CvCertificate> ReadAllCvCertificates();
    void RemoveCvCertificateById(CvCertificate cvCertificate);
    public InstituteOrSchool ReadInstituteOrSchoolById(long id);
    public InstituteOrSchool CreateInstituteOrSchool(InstituteOrSchool instituteOrSchool);
    IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools();
    IEnumerable<Certificate> ReadAllCertificates();
    public IEnumerable<CvCertificate> ReadCvCertificatesByPersonId(long personId);
    Certificate CreateCertificate(Certificate certificate); 



}