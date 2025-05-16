using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Shared.Types.Kernel.Types;

namespace LinguaPoint.Orders.Domain.Repositories;

/// <summary>
/// Repository interface for accessing and manipulating translation orders
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves a translation order by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translation order if found, null otherwise</returns>
    Task<TranslationOrder?> GetById(AggregateId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new translation order to the repository
    /// </summary>
    /// <param name="order">The order to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task Add(TranslationOrder order, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing translation order in the repository
    /// </summary>
    /// <param name="order">The order to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task Update(TranslationOrder order, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all translation orders for a specific client
    /// </summary>
    /// <param name="clientId">The unique identifier of the client</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of translation orders belonging to the client</returns>
    Task<IEnumerable<TranslationOrder>> GetByClientId(Guid clientId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all translation orders that are available for bidding
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of translation orders that are open for offers</returns>
    Task<IEnumerable<TranslationOrder>> GetAvailableForBidding(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all translation orders associated with a specific translator
    /// </summary>
    /// <param name="translatorId">The unique identifier of the translator</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of translation orders where the translator has submitted offers or is assigned</returns>
    Task<IEnumerable<TranslationOrder>> GetByTranslatorId(Guid translatorId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a specific translation file by its unique identifier
    /// </summary>
    /// <param name="fileId">The unique identifier of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translation file if found, null otherwise</returns>
    Task<TranslationFile?> GetFileById(Guid fileId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves orders that have been recently completed and need processing
    /// </summary>
    /// <param name="limit">Maximum number of orders to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of completed translation orders</returns>
    Task<IEnumerable<TranslationOrder>> GetRecentlyCompletedOrders(int limit = 10, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves orders where the translators need to be notified about new available work
    /// </summary>
    /// <param name="fromDate">The date from which to start looking for new orders</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of newly published translation orders</returns>
    Task<IEnumerable<TranslationOrder>> GetNewlyPublishedOrders(DateTime fromDate, CancellationToken cancellationToken = default);
}