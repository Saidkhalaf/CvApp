using cvapp.BL.Domain;
using cvapp.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace cvapp.DAL;

public class CvCertificateRepository : ICvCertificateRepository
{
    private CvAppDbContext _context;

    public CvCertificateRepository(CvAppDbContext context)
    {
        _context = context;
    }

    public CvCertificate CreateCvCertificate(CvCertificate cvCertificate)
    {
        _context.CvCertificates.Add(cvCertificate);
        _context.SaveChanges();
        return cvCertificate;
    }

    public CvCertificate UpdateCvCertificate(CvCertificate cvCertificate)
    {
        _context.CvCertificates.Update(cvCertificate);
        _context.SaveChanges();
        return cvCertificate;
    }

    public CvCertificate ReadCvCertificateById(long id)
    {
        return _context.CvCertificates.
            Include(c => c.CertificateName)
            .Include(c => c.InstituteOrSchool)
            .SingleOrDefault(c=> c.Id == id);
    }

    public Certificate ReadCertificateById(long certificateId)
    {
        return _context.Certificates.SingleOrDefault(c => c.Id == certificateId);
    }

    public IEnumerable<CvCertificate> ReadAllCvCertificates()
    {
        return _context.CvCertificates.Include(c=> c.CertificateName)
            .Include(c => c.InstituteOrSchool)
            .AsEnumerable();
    }

    public void RemoveCvCertificateById(CvCertificate certificate)
    {
        _context.CvCertificates.Remove(certificate);
        _context.SaveChanges();
    }

    public InstituteOrSchool ReadInstituteOrSchoolById(long id)
    {
        return _context.InstitutesOrSchools.SingleOrDefault(i => i.Id == id);
    }

    public InstituteOrSchool CreateInstituteOrSchool(InstituteOrSchool instituteOrSchool)
    {
        _context.InstitutesOrSchools.Add(instituteOrSchool);
        _context.SaveChanges();
        return instituteOrSchool;
    }

    public IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools()
    {
        return _context.InstitutesOrSchools.AsEnumerable();
    }

    public IEnumerable<Certificate> ReadAllCertificates()
    {
        return _context.Certificates.AsEnumerable();
    }

    public IEnumerable<CvCertificate> ReadCvCertificatesByPersonId(long personId)
    {
        return _context.CvCertificates
            .Include(c=> c.CertificateName)
            .Include(c=> c.InstituteOrSchool )
            .Where(c=> c.PersonId == personId)
            .ToList();
    }

    public Certificate CreateCertificate(Certificate certificate)
    {
         _context.Certificates.Add(certificate);
        _context.SaveChanges();
        return certificate;
    }
}