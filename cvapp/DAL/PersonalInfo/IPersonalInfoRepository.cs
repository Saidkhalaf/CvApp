using cvapp.BL.Domain;

namespace cvapp.DAL.PersonalInfo;

public interface IPersonalInfoRepository
{
    Person GetById(long id);
    Person Create(Person person);
    Person Update(Person person);
    Person ReadPersonWithTrainings(long personId);
}


