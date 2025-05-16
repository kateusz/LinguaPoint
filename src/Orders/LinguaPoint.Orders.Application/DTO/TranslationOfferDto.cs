namespace LinguaPoint.Orders.Application.DTO;

/// <summary>
/// Data Transfer Object for Translation Offers
/// </summary>
public class TranslationOfferDto
{
    /// <summary>
    /// Unique identifier of the offer
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Identifier of the translator who submitted this offer
    /// </summary>
    public Guid TranslatorId { get; set; }
    
    /// <summary>
    /// The translator's name, if available
    /// </summary>
    public string TranslatorName { get; set; }
    
    /// <summary>
    /// The price offered for the translation
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// The currency of the price
    /// </summary>
    public string Currency { get; set; }
    
    /// <summary>
    /// When the offer was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}