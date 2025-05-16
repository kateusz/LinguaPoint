namespace LinguaPoint.Users.Core.Models;

public class UserLanguage
{
    public string LanguageCode { get; private set; }
    public LanguageProficiency Proficiency { get; private set; }

    // For ORM
    private UserLanguage() { }

    public UserLanguage(string languageCode, LanguageProficiency proficiency)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code cannot be empty", nameof(languageCode));
        }

        LanguageCode = languageCode.ToLowerInvariant();
        Proficiency = proficiency;
    }

    public void UpdateProficiency(LanguageProficiency proficiency)
    {
        Proficiency = proficiency;
    }
}