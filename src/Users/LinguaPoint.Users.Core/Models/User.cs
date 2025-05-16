using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;

namespace LinguaPoint.Users.Core.Models;

public class User : AggregateRoot
{
    public Email Email { get; private set; }
    public FullName FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public List<UserLanguage> Languages { get; private set; } = new();
    public bool IsVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // For ORM
    private User() { }

    private User(
        AggregateId id,
        Email email,
        FullName fullName,
        string passwordHash,
        UserRole role)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        PasswordHash = passwordHash;
        Role = role;
        IsVerified = false;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddEvent(new UserCreated(Id, Email, FullName, Role));
    }

    public static User Create(
        Email email,
        FullName fullName,
        string passwordHash,
        UserRole role)
    {
        return new User(new AggregateId(), email, fullName, passwordHash, role);
    }

    public void VerifyEmail()
    {
        if (IsVerified)
        {
            return;
        }

        IsVerified = true;
        AddEvent(new UserEmailVerified(Id));
    }

    public void UpdateProfile(FullName? fullName = null)
    {
        if (fullName is not null && !fullName.Equals(FullName))
        {
            FullName = fullName;
        }

        AddEvent(new UserProfileUpdated(Id, FullName));
    }

    public void AddLanguage(string languageCode, LanguageProficiency proficiency)
    {
        if (Languages.Any(l => l.LanguageCode == languageCode))
        {
            throw new InvalidOperationException($"Language {languageCode} already exists for this user");
        }

        var userLanguage = new UserLanguage(languageCode, proficiency);
        Languages.Add(userLanguage);

        AddEvent(new UserLanguageAdded(Id, languageCode, proficiency));
    }

    public void UpdateLanguage(string languageCode, LanguageProficiency proficiency)
    {
        var language = Languages.FirstOrDefault(l => l.LanguageCode == languageCode);
        if (language is null)
        {
            throw new InvalidOperationException($"Language {languageCode} not found for this user");
        }

        language.UpdateProficiency(proficiency);
        AddEvent(new UserLanguageUpdated(Id, languageCode, proficiency));
    }

    public void RemoveLanguage(string languageCode)
    {
        var language = Languages.FirstOrDefault(l => l.LanguageCode == languageCode);
        if (language is null)
        {
            throw new InvalidOperationException($"Language {languageCode} not found for this user");
        }

        Languages.Remove(language);
        AddEvent(new UserLanguageRemoved(Id, languageCode));
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        AddEvent(new UserDeactivated(Id));
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        AddEvent(new UserReactivated(Id));
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        AddEvent(new UserPasswordChanged(Id));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AddEvent(new UserLoggedIn(Id));
    }
}