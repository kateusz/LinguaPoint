namespace LinguaPoint.Users.Core.DTO;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    IEnumerable<UserLanguageDto> Languages,
    bool IsVerified,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);