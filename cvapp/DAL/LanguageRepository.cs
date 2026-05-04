using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL.EF;

namespace cvapp.DAL;

public class LanguageRepository:ILanguageRepository
{
    private readonly CvAppDbContext _context;

    public LanguageRepository(CvAppDbContext context)
    {
        _context = context;
    }

    public Language ReadLanguage(long id)
    {
        return _context.Languages.Find(id);
    }

    public IEnumerable<Language> ReadAllLanguages()
    {
        return _context.Languages.AsEnumerable();
    }

    public IEnumerable<Language> ReadLanguagesOfLanguageLevel(LanguageLevel languageLevel)
    {
        return _context.Languages
            .Where(l => l.LanguageLevel == languageLevel)
            .AsEnumerable();
    }

    public IEnumerable<Language> ReadLanguagesOfLanguageName(LanguageName languageName)
    {
        return _context.Languages
            .Where(l => l.LanguageName == languageName)
            .AsEnumerable();
    }

    public IEnumerable<Language> ReadLanguagesOfPersonId(long personId)
    {
        return _context.Languages
            .Where(l => l.PersonId == personId)
            .AsEnumerable();
    }

    public bool ExistsByPersonAndLanguageName(long personId, LanguageName languageName)
    {
        return _context.Languages.Any(l=> l.PersonId == personId && l.LanguageName == languageName);
    }

    public void CreateLanguage(Language language)
    {
        _context.Languages.Add(language);
        _context.SaveChanges();
    }

    public void DeleteLanguage(Language language)
    {
        _context.Languages.Remove(language);
        _context.SaveChanges();
    }

    public Language UpdateLanguage(Language language)
    {
        _context.Languages.Update(language);
        _context.SaveChanges();
        return language;
    }

    public bool ExistsAnotherByPersonIdAndLanguageName(long languageId, long personId, LanguageName languageName)
    {
        return _context.Languages.Any(l => l.Id != languageId && l.PersonId == personId && l.LanguageName == languageName);
    }
}