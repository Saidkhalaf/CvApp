using cvapp.BL.Domain;
using cvapp.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace cvapp.DAL.PersonalInfo;

public class PersonalInfoRepository(CvAppDbContext dbContext) : IPersonalInfoRepository
{
    public Person GetById(long id)
    {
        return dbContext.Persons.Find(id);
    }

    public Person Create(Person person)
    {
        dbContext.Persons.Add(person);
        dbContext.SaveChanges();
        return person;
    }

    public Person Update(Person person)
    {
        dbContext.Persons.Update(person);
        dbContext.SaveChanges();
        return person;
    }

    public Person ReadPersonWithTrainings(long personId)
    {
        return dbContext.Persons
            .Include(p => p.CoursesTrainings)
            .ThenInclude(ct => ct.InstituteOrSchool)
            .Include(p => p.CoursesTrainings)
            .ThenInclude(ct => ct.Exam)
            .FirstOrDefault(p => p.Id == personId);
    }
}


