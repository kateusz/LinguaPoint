using System.ComponentModel.DataAnnotations;
using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Users.Core.Commands;


public record RegisterUserCommand(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(3)] [MaxLength(100)] string FullName,
    [Required] [MinLength(8)] string Password,
    [Required] string Role
) : ICommand;

public record LoginCommand(
    [Required] [EmailAddress] string Email,
    [Required] string Password
) : ICommand;

public record ChangePasswordCommand(
    Guid UserId,
    [Required] string CurrentPassword,
    [Required] [MinLength(8)] string NewPassword
) : ICommand;

public record AddUserLanguageCommand(
    Guid UserId,
    [Required] string LanguageCode,
    [Required] string Proficiency
) : ICommand;

public record VerifyEmailCommand(
    Guid UserId,
    [Required] string Token
) : ICommand;

public record DeactivateUserCommand(Guid UserId) : ICommand;

public record ReactivateUserCommand(Guid UserId) : ICommand;

public record RefreshTokenCommand(
    [Required] string RefreshToken
) : ICommand;