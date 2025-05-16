using System.ComponentModel.DataAnnotations;
using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Command to publish a translation order so it becomes available for bidding
/// </summary>
public record PublishOrderCommand : ICommand
{
    /// <summary>
    /// The unique identifier of the order to publish
    /// </summary>
    [Required]
    public Guid OrderId { get; init; }
    
    /// <summary>
    /// The client ID who is publishing the order (for verification)
    /// </summary>
    [Required]
    public Guid ClientId { get; init; }
}