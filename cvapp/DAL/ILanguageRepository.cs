using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace cvapp.DAL;

public interface ILanguageRepository
{
    Language ReadLanguage(long id);
    IEnumerable<Language> ReadAllLanguages();
    IEnumerable<Language> ReadLanguagesOfLanguageLevel(LanguageLevel languageLevel);
    IEnumerable<Language> ReadLanguagesOfLanguageName(LanguageName languageName);
    IEnumerable<Language> ReadLanguagesOfPersonId(long personId);
    bool ExistsByPersonAndLanguageName(long personId, LanguageName languageName);

    void CreateLanguage(Language language);
    void DeleteLanguage(Language language);
    Language UpdateLanguage(Language language);
    bool ExistsAnotherByPersonIdAndLanguageName(long languageId, long personId, LanguageName languageName);
}