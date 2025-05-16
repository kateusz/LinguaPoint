using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Queries;
using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Users.Core.DTO;
using LinguaPoint.Users.Core.Models;
using LinguaPoint.Users.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Users.Core.Queries;

public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdHandler> _logger;

    public GetUserByIdHandler(IUserRepository userRepository, ILogger<GetUserByIdHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetById(new AggregateId(query.Id), cancellationToken);
            if (user is null)
            {
                return Result<UserDto>.Failure("User not found");
            }

            return Result<UserDto>.Success(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", query.Id);
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