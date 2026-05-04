using cvapp.BL.Domain;
using cvapp.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace cvapp.DAL;

public class CvEducationRepository : ICvEducationRepository
{
    private readonly CvAppDbContext _context;

    public CvEducationRepository(CvAppDbContext context)
    {
        _context = context;
    }

    public CvEducation CreateCvEducation(CvEducation cvEducation)
    {
        _context.CvEducations.Add(cvEducation);
        _context.SaveChanges();
        return cvEducation;
    }

    public CvEducation ReadCvEducationById(long id)
    {
        return _context.CvEducations
            .Include(e => e.Person)
            .Include(e => e.Major)
           .ThenInclude(m=> m.EducationLevel)
            .Include(e => e.InstituteOrSchool)
            .SingleOrDefault(e => e.Id == id);
    }

    public IEnumerable<CvEducation> ReadCvEducationsByPersonId(long personId)
    {
        return _context.CvEducations
            .Include(e => e.Major)
            .ThenInclude(m => m.EducationLevel)
            .Include(e => e.InstituteOrSchool)
            .Where(e => e.PersonId == personId)
            .OrderByDescending(e => e.EndYear)
            .ThenByDescending(e => e.StartYear)
            .ToList();
    }

    public IEnumerable<Education> ReadAllEducations()
    {
        return _context.Educations
            .Include<Education, EducationOptions>(e => e.EducationLevel)
            .OrderBy(e => e.Name)
            .ToList();
    }

    public Education ReadEducationById(long id)
    {
        return _context.Educations.SingleOrDefault(e => e.Id == id);
    }

    public IEnumerable<InstituteOrSchool> ReadAllInstituteOrSchools()
    {
        return _context.InstitutesOrSchools
            .OrderBy(i => i.Name)
            .ToList();
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

    public bool InstituteOrSchoolNameExists(string name)
    {
        
        return _context.InstitutesOrSchools.Any(i => i.Name == name);
    }

    public void UpdateCvEducation(CvEducation cvEducation)
    {
        _context.CvEducations.Update(cvEducation);
        _context.SaveChanges();
    }

    public void DeleteCvEducation(CvEducation cvEducation)
    {
        _context.CvEducations.Remove(cvEducation);
        _context.SaveChanges();
    }
}
