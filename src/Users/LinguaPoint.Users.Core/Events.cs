using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;
using LinguaPoint.Users.Core.Models;

namespace LinguaPoint.Users.Core;

public record UserCreated(
    Guid UserId,
    Email Email,
    FullName FullName,
    UserRole Role
) : IDomainEvent;

public record UserEmailVerified(
    Guid UserId
) : IDomainEvent;

public record UserProfileUpdated(
    Guid UserId,
    FullName? FullName = null,
    string? Bio = null,
    string? ProfilePictureUrl = null
) : IDomainEvent;

public record UserLanguageAdded(
    Guid UserId,
    string LanguageCode,
    LanguageProficiency Proficiency
) : IDomainEvent;

public record UserLanguageUpdated(
    Guid UserId,
    string LanguageCode,
    LanguageProficiency Proficiency
) : IDomainEvent;

public record UserLanguageRemoved(
    Guid UserId,
    string LanguageCode
) : IDomainEvent;

public record UserDeactivated(
    Guid UserId
) : IDomainEvent;

public record UserReactivated(
    Guid UserId
) : IDomainEvent;

public record UserPasswordChanged(
    Guid UserId
) : IDomainEvent;

public record UserLoggedIn(
    Guid UserId
) : IDomainEvent;