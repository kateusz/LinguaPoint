using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Shared.Types.Kernel;

namespace LinguaPoint.Orders.Domain.Shared.DomainEvents;

public record OrderCreated(Guid OrderId, Guid ClientId, LanguagePair LangPair) : IDomainEvent;
public record FileUploaded(Guid OrderId, Guid FileId, string FileName) : IDomainEvent;
public record FileProcessed(Guid OrderId, Guid FileId) : IDomainEvent;
public record FileProcessingFailed(Guid OrderId, Guid FileId, string Reason) : IDomainEvent;
public record OrderPublished(Guid OrderId, Guid ClientId, string LanguagePair, DateTime PublishedAt) : IDomainEvent;
public record TranslationOfferSubmitted(
    Guid OrderId,
    Guid TranslatorId,
    decimal Price,
    string Currency,
    DateTime CreatedAt
) : IDomainEvent;

public record TranslationOfferAccepted(
    Guid OrderId,
    Guid TranslatorId,
    decimal Price,
    string Currency,
    DateTime AcceptedAt
) : IDomainEvent;

public record TranslationStarted(
    Guid OrderId,
    Guid TranslatorId,
    DateTime StartedAt
) : IDomainEvent;

public record TranslationDelivered(
    Guid OrderId,
    Guid FileId,
    Guid TranslatorId,
    string TranslatedFilePath,
    DateTime DeliveredAt
) : IDomainEvent;

public record TranslationApproved(
    Guid OrderId,
    Guid FileId,
    DateTime ApprovedAt
) : IDomainEvent;

public record TranslationRevisionRequested(
    Guid OrderId,
    Guid FileId,
    string Comment,
    DateTime RequestedAt
) : IDomainEvent;

public record TranslationRevisionDelivered(
    Guid OrderId,
    Guid FileId,
    string NewTranslatedFilePath,
    DateTime DeliveredAt
) : IDomainEvent;

public record TranslationOrderCompleted(
    Guid OrderId,
    DateTime CompletedAt
) : IDomainEvent;
