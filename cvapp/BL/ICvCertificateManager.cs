using System;
using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace BL;

public interface ICvCertificateManager
{
    CvCertificate AddCvCertificate(int year, long certificateId, long instituteOrSchoolId,
        string certificateCode, string uploadedCertificate, DateTime expiryDate , long personId);

    CvCertificate UpdateCvCertificate(long id , int year, long certificateId, long instituteOrSchoolId,
        string certificateCode,
        string uploadedCertificate, DateTime expiryDate, long personId);

  void DeleteCvCertificate(long id);

 CvCertificate GetCvCertificateById(long id);
  IEnumerable<CvCertificate> GetAllCvCertificates();
  IEnumerable<Certificate> GetAllCertificates();
    IEnumerable<InstituteOrSchool> GetAllInstituteOrSchools();
    IEnumerable<CvCertificate> GetCvCertificatesByPersonId(long personId);
    Certificate AddCertificate(string name, CertificateType type);

}