using cvapp.BL.Domain.enums;
using cvapp.BL.Domain;

namespace BL.PersonalInfo;

public interface IPersonalInfoManager
{
    Person GetById(long id);
    Person Add(
        long personId,
        string firstName,
        string lastName,
        DateTime? dateOfBirth,
        Gender? gender,
        MaritalStatus? maritalStatus,
        string street,
        string houseNumber,
        string bus,
        string postalCode,
        string city,
        string country,
        string email,
        string phone,
        string linkedInUrl, 
        string jobTitle,
        string summary);
    
    Person GetPersonWithTrainings(long personId);
}



