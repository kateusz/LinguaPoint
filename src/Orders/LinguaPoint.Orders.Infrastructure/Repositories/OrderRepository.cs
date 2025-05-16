using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Orders.Domain.Repositories;
using LinguaPoint.Orders.Infrastructure.Persistence;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Orders.Infrastructure.Repositories;


/// <summary>
/// Implementation of the IOrderRepository interface using Entity Framework Core
/// </summary>
internal class OrderRepository : IOrderRepository
{
    private readonly OrdersContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrdersContext context, ILogger<OrderRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a translation order by its unique identifier, including all related entities
    /// </summary>
    public async Task<TranslationOrder?> GetById(AggregateId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Include("_offers") // This accesses the backing field for the Offers collection
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Adds a new translation order to the database
    /// </summary>
    public async Task Add(TranslationOrder order, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new translation order to database for client {ClientId}", order.ClientId);
            await _context.TranslationOrders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully added order with ID {OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order for client {ClientId}", order.ClientId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing translation order in the database
    /// </summary>
    public async Task Update(TranslationOrder order, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating translation order {OrderId}", order.Id);
            
            // Ensure the entity is being tracked
            _context.TranslationOrders.Update(order);
            
            // Save changes to the database
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully updated order {OrderId}", order.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict when updating order {OrderId}", order.Id);
            
            // Handle the concurrency conflict appropriately
            // You might want to implement optimistic concurrency retry logic here
            throw new InvalidOperationException($"The order has been modified by another user. Please refresh and try again.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", order.Id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all translation orders for a specific client
    /// </summary>
    public async Task<IEnumerable<TranslationOrder>> GetByClientId(Guid clientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving orders for client {ClientId}", clientId);
            
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for client {ClientId}", clientId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all translation orders that are available for bidding
    /// </summary>
    public async Task<IEnumerable<TranslationOrder>> GetAvailableForBidding(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all orders available for bidding");
            
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Where(o => o.Status == OrderStatus.AvailableForBidding)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders available for bidding");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all translation orders associated with a specific translator
    /// </summary>
    public async Task<IEnumerable<TranslationOrder>> GetByTranslatorId(Guid translatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving orders for translator {TranslatorId}", translatorId);
            
            // This query needs to check orders where:
            // 1. The translator has submitted an offer
            // 2. The translator's offer was accepted
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Include("_offers")
                .Where(o => 
                    // Check if this translator's offer was accepted
                    EF.Property<Guid?>(o, "AcceptedOfferId") != null && 
                    o.Offers.Any(of => of.Id == EF.Property<Guid?>(o, "AcceptedOfferId") && of.TranslatorId == translatorId)
                    || 
                    // Or if this translator has submitted any offer for this order
                    o.Offers.Any(of => of.TranslatorId == translatorId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for translator {TranslatorId}", translatorId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific translation file by its unique identifier
    /// </summary>
    public async Task<TranslationFile?> GetFileById(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving file with ID {FileId}", fileId);
            
            // First, find the order that contains this file
            var order = await _context.TranslationOrders
                .Include(o => o.Files)
                .FirstOrDefaultAsync(o => o.Files.Any(f => f.Id == fileId), cancellationToken);

            // Then return the specific file
            return order?.Files.FirstOrDefault(f => f.Id == fileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file with ID {FileId}", fileId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves orders that have been recently completed and need processing
    /// </summary>
    public async Task<IEnumerable<TranslationOrder>> GetRecentlyCompletedOrders(int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving recently completed orders (limit: {Limit})", limit);
            
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Include("_offers")
                .Where(o => o.Status == OrderStatus.Completed)
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recently completed orders");
            throw;
        }
    }

    /// <summary>
    /// Retrieves orders where the translators need to be notified about new available work
    /// </summary>
    public async Task<IEnumerable<TranslationOrder>> GetNewlyPublishedOrders(DateTime fromDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving newly published orders since {FromDate}", fromDate);
            
            // Find orders that have been published after the specified date
            // The domain events would have timestamps in them, but for a quick query we can use CreatedAt
            // A more accurate approach would be to add a PublishedAt field to the order entity
            return await _context.TranslationOrders
                .Include(o => o.Files)
                .Where(o => o.Status == OrderStatus.AvailableForBidding && o.CreatedAt >= fromDate)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving newly published orders since {FromDate}", fromDate);
            throw;
        }
    }
}