using System.ComponentModel.DataAnnotations;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL.PersonalInfo;

namespace BL.PersonalInfo;

public class PersonalInfoManager(IPersonalInfoRepository repository) : IPersonalInfoManager
{
    public Person GetById(long id)
    {
        return repository.GetById(id);
    }

    public Person Add(
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
        string jobTitle, string summary)
    {
        var effectivePersonId = personId > 0 ? personId : AppDefaults.DefaultPersonId;

        var person = repository.GetById(effectivePersonId);
        var exists = person != null;
        if (!exists)
        {
            person = new Person { Id = effectivePersonId };
        }

        person.FirstName = Normalize(firstName);
        person.LastName = Normalize(lastName);
        person.DateOfBirth = dateOfBirth;
        person.Gender = gender ?? Gender.Other;
        person.MaritalStatus = maritalStatus ?? MaritalStatus.Single;
        person.Street = Normalize(street);
        person.HouseNumber = Normalize(houseNumber);
        person.Bus = Normalize(bus);
        person.PostalCode = Normalize(postalCode);
        person.City = Normalize(city);
        person.Country = Normalize(country);
        person.Email = Normalize(email);
        person.Phone = Normalize(phone);
        person.LinkedInUrl = Normalize(linkedInUrl);
        person.JobTitle = Normalize(jobTitle);
        person.Summary = Normalize(summary);

        Validator.ValidateObject(person, new ValidationContext(person), validateAllProperties: true);

        if (!exists)
        {
            repository.Create(person);
            return person;
        }

        repository.Update(person);
        return person;
    }

    public Person GetPersonWithTrainings(long personId)
    {
        return repository.ReadPersonWithTrainings(personId);
    }


    private static string Normalize(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}


