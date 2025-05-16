namespace LinguaPoint.Orders.Application.DTO;

/// <summary>
/// Data Transfer Object for Translation Files
/// </summary>
public class TranslationFileDto
{
    /// <summary>
    /// Unique identifier of the file
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The original filename as uploaded by the client
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Current processing status of the file
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Error message if the file processing failed
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Path to the translated file (if delivered)
    /// </summary>
    public string TranslatedFilePath { get; set; }

    /// <summary>
    /// Whether the translation has been delivered
    /// </summary>
    public bool IsDelivered { get; set; }

    /// <summary>
    /// Current review status of the translation
    /// </summary>
    public string ReviewStatus { get; set; }

    /// <summary>
    /// Comments for revision requests
    /// </summary>
    public string RevisionComment { get; set; }
}