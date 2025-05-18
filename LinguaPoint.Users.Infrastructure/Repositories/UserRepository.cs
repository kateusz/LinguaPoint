using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;
using LinguaPoint.Users.Core.Models;
using LinguaPoint.Users.Core.Repositories;
using LinguaPoint.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LinguaPoint.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersContext _dbContext;

    public UserRepository(UsersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetById(AggregateId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmail(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task Add(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExists(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetTranslatorsWithLanguage(string languageCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Languages)
            .Where(u => u.Role == UserRole.Translator &&
                        u.IsActive &&
                        u.Languages.Any(l => l.LanguageCode == languageCode))
            .ToListAsync(cancellationToken);
    }
}