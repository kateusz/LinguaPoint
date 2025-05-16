using System.ComponentModel.DataAnnotations;
using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Orders.Application.Commands;
/// <summary>
/// Command to create a new translation order in the system
/// </summary>
public record CreateTranslationOrderCommand : ICommand
{
    /// <summary>
    /// The unique identifier of the client creating the order
    /// </summary>
    [Required]
    public Guid ClientId { get; init; }
    
    /// <summary>
    /// The source language for the translation (e.g., "en", "fr", "es")
    /// </summary>
    [Required]
    [StringLength(10)]
    public string SourceLanguage { get; init; }
    
    /// <summary>
    /// The target language for the translation (e.g., "en", "fr", "es")
    /// </summary>
    [Required]
    [StringLength(10)]
    public string TargetLanguage { get; init; }
    
    /// <summary>
    /// Optional initial file path to upload with the order
    /// If provided, the file will be attached to the order immediately
    /// </summary>
    public string? InitialFilePath { get; init; }
    
    /// <summary>
    /// Optional original filename of the initial upload
    /// Required if InitialFilePath is provided
    /// </summary>
    public string? InitialFileName { get; init; }
}