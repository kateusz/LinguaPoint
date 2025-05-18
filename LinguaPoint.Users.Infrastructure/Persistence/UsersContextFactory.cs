using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LinguaPoint.Users.Infrastructure.Persistence
{
    public class UsersContextFactory : IDesignTimeDbContextFactory<UsersContext>
    {
        public UsersContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UsersContext>();
            optionsBuilder.UseSqlServer("");
            return new UsersContext(optionsBuilder.Options);
        }
    }
}