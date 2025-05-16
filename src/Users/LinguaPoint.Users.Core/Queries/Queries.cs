using LinguaPoint.Shared.Queries;
using LinguaPoint.Users.Core.DTO;

namespace LinguaPoint.Users.Core.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;

public record GetUserByEmailQuery(string Email) : IQuery<UserDto>;

public record GetUserLanguagesQuery(Guid UserId) : IQuery<IEnumerable<UserLanguageDto>>;

public record GetTranslatorsWithLanguageQuery(string LanguageCode) : IQuery<IEnumerable<UserDto>>;

public record GetCurrentUserQuery(Guid UserId) : IQuery<UserDto>;