using System.ComponentModel.DataAnnotations;
using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Command to add a file to an existing translation order
/// </summary>
public record AddFileToOrderCommand : ICommand
{
    /// <summary>
    /// The unique identifier of the order to add the file to
    /// </summary>
    [Required]
    public Guid OrderId { get; init; }
    
    /// <summary>
    /// The file path where the uploaded file is temporarily stored
    /// </summary>
    [Required]
    public string FilePath { get; init; }
    
    /// <summary>
    /// The original filename as uploaded by the client
    /// </summary>
    [Required]
    public string FileName { get; init; }
    
    /// <summary>
    /// The client ID who is adding the file (for verification)
    /// </summary>
    [Required]
    public Guid ClientId { get; init; }
}