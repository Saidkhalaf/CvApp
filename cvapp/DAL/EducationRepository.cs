using cvapp.BL.Domain;
using cvapp.DAL.EF;

namespace cvapp.DAL;

public class EducationRepository : IEducationRepository
{
    private readonly CvAppDbContext _context;

    public EducationRepository(CvAppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Education> ReadAll()
    {
        return _context.Educations
            .OrderBy(e => e.Name)
            .ToList();
    }

    public Education ReadById(long id)
    {
        return _context.Educations.SingleOrDefault(e => e.Id == id);
    }

    public Education Create(Education education)
    {
        _context.Educations.Add(education);
        _context.SaveChanges();
        return education;
    }

    public IEnumerable<EducationOptions> ReadAllEducationOptions()
    {
        return _context.EducationOptions.OrderBy(e=>e.EducationLevel).ToList();
    }

    public EducationOptions ReadEducationOptionsById(long id)
    {
       return _context.EducationOptions.SingleOrDefault(e=>e.Id == id);
    }
}