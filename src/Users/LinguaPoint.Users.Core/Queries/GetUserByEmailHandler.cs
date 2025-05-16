using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Queries;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;
using LinguaPoint.Users.Core.DTO;
using LinguaPoint.Users.Core.Models;
using LinguaPoint.Users.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Users.Core.Queries;

public class GetUserByEmailHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByEmailHandler> _logger;

    public GetUserByEmailHandler(IUserRepository userRepository, ILogger<GetUserByEmailHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmail(new Email(query.Email), cancellationToken);
            if (user is null)
            {
                return Result<UserDto>.Failure("User not found");
            }

            return Result<UserDto>.Success(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", query.Email);
            return Result<UserDto>.Failure($"Error getting user: {ex.Message}");
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            user.Languages.Select(l => new UserLanguageDto(l.LanguageCode, l.Proficiency)),
            user.IsVerified,
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt
        );
    }
}