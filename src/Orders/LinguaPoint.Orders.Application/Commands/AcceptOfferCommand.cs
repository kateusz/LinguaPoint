using System.ComponentModel.DataAnnotations;
using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Command to accept a translator's offer for an order
/// </summary>
public record AcceptOfferCommand : ICommand
{
    /// <summary>
    /// The unique identifier of the order
    /// </summary>
    [Required]
    public Guid OrderId { get; init; }
    
    /// <summary>
    /// The translator's ID whose offer is being accepted
    /// </summary>
    [Required]
    public Guid TranslatorId { get; init; }
    
    /// <summary>
    /// The client ID who is accepting the offer (for verification)
    /// </summary>
    [Required]
    public Guid ClientId { get; init; }
}