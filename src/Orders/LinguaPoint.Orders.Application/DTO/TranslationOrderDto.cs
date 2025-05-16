namespace LinguaPoint.Orders.Application.DTO;


/// <summary>
/// Data Transfer Object for Translation Orders
/// </summary>
public class TranslationOrderDto
{
    /// <summary>
    /// Unique identifier of the order
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Identifier of the client who created the order
    /// </summary>
    public Guid ClientId { get; set; }
    
    /// <summary>
    /// Source language code for the translation
    /// </summary>
    public string SourceLanguage { get; set; }
    
    /// <summary>
    /// Target language code for the translation
    /// </summary>
    public string TargetLanguage { get; set; }
    
    /// <summary>
    /// Current status of the order
    /// </summary>
    public string Status { get; set; }
    
    /// <summary>
    /// Price of the order, if set
    /// </summary>
    public decimal? Amount { get; set; }
    
    /// <summary>
    /// Currency of the price, if set
    /// </summary>
    public string Currency { get; set; }
    
    /// <summary>
    /// When the order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Files attached to this order
    /// </summary>
    public List<TranslationFileDto> Files { get; set; } = new();
    
    /// <summary>
    /// Offers submitted for this order
    /// </summary>
    public List<TranslationOfferDto> Offers { get; set; } = new();
    
    /// <summary>
    /// The accepted offer, if any
    /// </summary>
    public TranslationOfferDto AcceptedOffer { get; set; }
}