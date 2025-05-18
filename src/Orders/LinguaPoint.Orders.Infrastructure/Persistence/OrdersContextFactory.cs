using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LinguaPoint.Orders.Infrastructure.Persistence;

public class OrdersContextFactory : IDesignTimeDbContextFactory<OrdersContext>
{
    public OrdersContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
        optionsBuilder.UseSqlServer("");
        return new OrdersContext(optionsBuilder.Options);
    }
}