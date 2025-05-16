using LinguaPoint.Shared.Types.Kernel.ValueObjects;

namespace LinguaPoint.Orders.Domain.Orders;

public class TranslationOffer
{
    public Guid Id { get; }
    public Guid TranslatorId { get; }
    public Money Price { get; }
    public DateTime CreatedAt { get; }

    public TranslationOffer(Guid id, Guid translatorId, Money price, DateTime createdAt)
    {
        Id = id;
        TranslatorId = translatorId;
        Price = price;
        CreatedAt = createdAt;
    }
}