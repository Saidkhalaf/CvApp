using System;
using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL;

namespace BL;

public class CvCertificateManager : ICvCertificateManager
{
    private readonly ICvCertificateRepository _repository;

    public CvCertificateManager(ICvCertificateRepository repository)
    {
        _repository = repository;
    }
    public CvCertificate AddCvCertificate(int year, long certificateId, long instituteOrSchoolId, string certificateCode,
        string uploadedCertificate, DateTime expiryDate , long personId)
    {
        var certificate = _repository.ReadCertificateById(certificateId);


        if (certificate ==  null)
        {
            throw new ArgumentException("invalid certificate id");
        }
        var institute = _repository.ReadInstituteOrSchoolById(instituteOrSchoolId);
        if (institute==null)
        {
            throw new ArgumentException("Invalid institute id"); 
        }
        CvCertificate cvCertificate = new CvCertificate(year, certificateId,instituteOrSchoolId,certificateCode,uploadedCertificate,expiryDate,personId);
       return  _repository.CreateCvCertificate(cvCertificate);

    }

    public CvCertificate UpdateCvCertificate(long id ,int year, long certificateId, long instituteOrSchoolId, string certificateCode,
        string uploadedCertificate, DateTime expiryDate, long personId)
    {
        var cvCertificate = _repository.ReadCvCertificateById(id);

        if (cvCertificate == null)
        {
            throw new ArgumentException("Invalid Cvcertificate Id");
        }  
        var institute = _repository.ReadInstituteOrSchoolById(instituteOrSchoolId);
        if (institute == null)
        {
            throw new ArgumentException("Invalid institute Id");
        }
        var certificate = _repository.ReadCertificateById(certificateId);
        if (certificate == null)
        {
            throw new ArgumentException("Invalid certificate Id");
        }

        cvCertificate.CertificateId = certificateId;
        cvCertificate.Certificate = uploadedCertificate;
        cvCertificate.CertificateCode = certificateCode;
        cvCertificate.InstituteOrSchoolId = instituteOrSchoolId;
        cvCertificate.ExpiresOn = expiryDate;
        cvCertificate.PersonId = personId;
        cvCertificate.Year = year;

        return _repository.UpdateCvCertificate(cvCertificate);
        

    }

    public void DeleteCvCertificate(long id)
    {
        var cvCertificate = _repository.ReadCvCertificateById(id);
        if (cvCertificate == null)
        {
            throw new ArgumentException("cvCertificate not found");
        }
        _repository.RemoveCvCertificateById(cvCertificate);
    }

    public CvCertificate GetCvCertificateById(long id)
    {
        return _repository.ReadCvCertificateById(id);
    }

    public IEnumerable<CvCertificate> GetAllCvCertificates()
    {
        return _repository.ReadAllCvCertificates();
    }

    public IEnumerable<Certificate> GetAllCertificates()
    {
        return _repository.ReadAllCertificates();
    }

    public IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools()
    {
        return _repository.ReadAllInstituteOrSchools();
    }

    public IEnumerable<CvCertificate> GetCvCertificatesByPersonId(long personId)
    {
        return _repository.ReadCvCertificatesByPersonId(personId);
    }

    public Certificate AddCertificate(string name, CertificateType type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Certificate name cannot be empty");
        }
        
        var certificate = new Certificate
        {
            Name = name,
            Type = type
        };
        return _repository.CreateCertificate(certificate);
    }
}