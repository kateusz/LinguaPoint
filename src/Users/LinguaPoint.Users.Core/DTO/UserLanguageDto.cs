using LinguaPoint.Users.Core.Models;

namespace LinguaPoint.Users.Core.DTO;
public record UserLanguageDto(
    string LanguageCode,
    LanguageProficiency Proficiency
);