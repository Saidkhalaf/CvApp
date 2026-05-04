using System;
using System.Collections.Generic;
using cvapp.BL.Domain;
using cvapp.BL.Domain.enums;
using cvapp.DAL;
using Language = cvapp.BL.Domain.Language;

namespace BL;

public class LanguageManager : ILanguageManager
{
    private readonly ILanguageRepository _languageRepository;

    public LanguageManager(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;
    }

    public IEnumerable<Language> GetAlLanguages()
    {
        return _languageRepository.ReadAllLanguages();
    }

    public Language GetLanguageById(long id)
    {
        return _languageRepository.ReadLanguage(id);
    }

    public IEnumerable<Language> GetLanguagesByPersonId(long personId)
    {
        return _languageRepository.ReadLanguagesOfPersonId(personId);
    }

    public Language AddLanguage(LanguageName languageName, LanguageLevel languagelevel)
    {
        var personId = AppDefaults.DefaultPersonId;

        var exists = _languageRepository.ExistsByPersonAndLanguageName(personId, languageName);
        if (exists)
        {
            throw new InvalidOperationException("This language already exists for this person.");
        }

        Language createdLanguage = new Language(personId, languagelevel, languageName);
        _languageRepository.CreateLanguage(createdLanguage);
        return createdLanguage;
    }

    public void RemoveLanguage(long id)
    {
        var language = _languageRepository.ReadLanguage(id);
        if (language == null)
        {
            throw new InvalidOperationException("Language not found.");
        }

        _languageRepository.DeleteLanguage(language);
    }

    public Language EditLanguage(long id, LanguageName languageName, LanguageLevel languagelevel)
    {
        var languageToEdit = _languageRepository.ReadLanguage(id);
        var personId = AppDefaults.DefaultPersonId;
        if (languageToEdit == null)
        {
            throw new InvalidOperationException("Language not found.");
        }

        var exists = _languageRepository.ExistsAnotherByPersonIdAndLanguageName(id, personId, languageName);
        if (exists)
        {
            throw new InvalidOperationException("This language already exists for this person.");
        }

        languageToEdit.LanguageName = languageName;
        languageToEdit.LanguageLevel = languagelevel;

        return _languageRepository.UpdateLanguage(languageToEdit);
    }
}
