using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;
using LinguaPoint.Users.Core.Models;

namespace LinguaPoint.Users.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetById(AggregateId id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmail(Email email, CancellationToken cancellationToken = default);
    Task Add(User user, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
    Task<bool> EmailExists(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetTranslatorsWithLanguage(string languageCode, CancellationToken cancellationToken = default);
}