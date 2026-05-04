using cvapp.BL.Domain.enums;

namespace cvapp.BL.Domain;

public class Language
{
    public long Id { get; set; }
    public Person Person { get; set; }
    public long PersonId { get; set; }
    public LanguageLevel LanguageLevel { get; set; }
    public LanguageName LanguageName { get; set; }
    
    public Language()
    {
        
    }
    
    public Language( long personId, LanguageLevel languageLevel, LanguageName languageName)
    {
        

        PersonId = personId;
        LanguageLevel = languageLevel;
        LanguageName = languageName;
    }
}