using System.Security.Cryptography;
using System.Text;
using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;
using LinguaPoint.Users.Core.DTO;
using LinguaPoint.Users.Core.Models;
using LinguaPoint.Users.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Users.Core.Commands;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher,
        ILogger<RegisterUserHandler> logger)
    {
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new Email(command.Email);
            if (await _userRepository.EmailExists(email, cancellationToken))
            {
                return Result<UserDto>.Failure("Email already exists");
            }
            
            if (!Enum.TryParse<UserRole>(command.Role, true, out var role))
            {
                return Result<UserDto>.Failure("Invalid user role");
            }
            
            var passwordHash = HashPassword(command.Password);
            var user = User.Create(
                email,
                new FullName(command.FullName),
                passwordHash,
                role);
            
            await _userRepository.Add(user, cancellationToken);
            
            foreach (var @event in user.Events)
            {
                await _eventDispatcher.DispatchAsync(@event, cancellationToken);
            }

            return Result<UserDto>.Success(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", command.Email);
            return Result<UserDto>.Failure($"Error registering user: {ex.Message}");
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
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