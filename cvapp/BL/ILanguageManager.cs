using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;

namespace BL;

public interface ILanguageManager
{
    IEnumerable<Language> GetAlLanguages();
    Language GetLanguageById(long id);
    IEnumerable<Language> GetLanguagesByPersonId(long personId);
    Language AddLanguage(LanguageName languageName, LanguageLevel languagelevel);
    void RemoveLanguage(long id);
    Language EditLanguage(long id, LanguageName language, LanguageLevel languagelevel);

}