using LinguaPoint.Orders.Domain.Shared.DomainEvents;
using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Shared.Types.Kernel.ValueObjects;

namespace LinguaPoint.Orders.Domain.Orders;

public class TranslationOrder : AggregateRoot
{
    private readonly List<TranslationFile> _files = [];
    private readonly List<TranslationOffer> _offers = [];
    private TranslationOffer? _acceptedOffer;
    public Guid ClientId { get; }
    public LanguagePair LanguagePair { get; }
    public OrderStatus Status { get; private set; }
    public Money? Price { get; private set; }
    public DateTime CreatedAt { get; }

    public IReadOnlyCollection<TranslationFile> Files => _files.AsReadOnly();
    public IReadOnlyCollection<TranslationOffer> Offers => _offers.AsReadOnly();
    public TranslationOffer? AcceptedOffer => _acceptedOffer;

    private TranslationOrder()
    {
    }

    private TranslationOrder(AggregateId id, Guid clientId, LanguagePair langPair)
    {
        Id = id;
        ClientId = clientId;
        LanguagePair = langPair;
        Status = OrderStatus.New;
        CreatedAt = DateTime.UtcNow;

        AddEvent(new OrderCreated(Id, ClientId, langPair));
    }

    public static TranslationOrder Create(Guid clientId, LanguagePair languagePair)
    {
        var id = new AggregateId();
        return new TranslationOrder(id, clientId, languagePair);
    }

    public void AddFile(string filePath, string fileName)
    {
        var file = new TranslationFile(Guid.NewGuid(), Id, fileName, filePath);
        _files.Add(file);

        AddEvent(new FileUploaded(Id, file.Id, file.FileName));
    }

    public void MarkFileAsProcessed(Guid fileId)
    {
        var file = _files.FirstOrDefault(f => f.Id == fileId)
                   ?? throw new InvalidOperationException("File not found");

        file.MarkAsProcessed();

        AddEvent(new FileProcessed(Id, fileId));
    }

    public void MarkFileAsFailed(Guid fileId, string reason)
    {
        var file = _files.FirstOrDefault(f => f.Id == fileId)
                   ?? throw new InvalidOperationException("File not found");

        file.MarkAsFailed(reason);

        AddEvent(new FileProcessingFailed(Id, fileId, reason));
    }

    public void PublishOrder()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Only new orders can be published.");

        if (_files.Count == 0)
            throw new InvalidOperationException("Cannot publish order without uploaded files.");

        Status = OrderStatus.AvailableForBidding;

        AddEvent(new OrderPublished(Id, ClientId, LanguagePair.ToString(), DateTime.UtcNow));
    }

    public void SubmitOffer(Guid translatorId, Money price)
    {
        if (Status != OrderStatus.AvailableForBidding)
            throw new InvalidOperationException("Cannot submit offer for this order.");

        if (_offers.Exists(x => x.TranslatorId == translatorId))
            throw new InvalidOperationException("You have already submitted an offer for this order.");

        var offer = new TranslationOffer(Guid.NewGuid(), translatorId, price, DateTime.UtcNow);
        _offers.Add(offer);

        AddEvent(new TranslationOfferSubmitted(Id, translatorId, price.Amount, price.Currency, offer.CreatedAt));
    }

    public void AcceptOffer(Guid translatorId)
    {
        if (Status != OrderStatus.AvailableForBidding)
            throw new InvalidOperationException("Cannot accept offer in the current order state.");

        var offer = _offers.Find(x => x.TranslatorId == translatorId);
        if (offer is null)
            throw new InvalidOperationException("No offer from this translator was found.");

        _acceptedOffer = offer;
        Status = OrderStatus.OfferAccepted;

        AddEvent(new TranslationOfferAccepted(Id, translatorId, offer.Price.Amount, offer.Price.Currency,
            DateTime.UtcNow));
    }

    public void MarkInProgress(Guid translatorId)
    {
        if (Status != OrderStatus.OfferAccepted)
            throw new InvalidOperationException("Cannot start translation in the current state.");

        if (_acceptedOffer is null || _acceptedOffer.TranslatorId != translatorId)
            throw new InvalidOperationException("Only the accepted translator can start this translation.");

        Status = OrderStatus.InProgress;

        AddEvent(new TranslationStarted(Id, translatorId, DateTime.UtcNow));
    }

    public void DeliverTranslation(Guid translatorId, Guid fileId, string translatedFilePath)
    {
        if (Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Order is not in progress.");

        if (_acceptedOffer?.TranslatorId != translatorId)
            throw new InvalidOperationException("Only the assigned translator can deliver the translation.");

        var file = _files.FirstOrDefault(f => f.Id == fileId);
        if (file is null)
            throw new InvalidOperationException("File not found in order.");

        file.DeliverTranslatedFile(translatedFilePath);

        // Jeśli wszystkie przetłumaczone:
        if (_files.All(f => f.IsDelivered))
        {
            Status = OrderStatus.Delivered;
        }

        AddEvent(new TranslationDelivered(Id, file.Id, translatorId, translatedFilePath, DateTime.UtcNow));
    }

    public void ApproveTranslation(Guid fileId)
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot approve file before order is delivered.");

        var file = _files.FirstOrDefault(f => f.Id == fileId)
                   ?? throw new InvalidOperationException("File not found in order.");

        file.Approve();

        AddEvent(new TranslationApproved(Id, fileId, DateTime.UtcNow));
    }

    public void RequestRevision(Guid fileId, string comment)
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot request revision before order is delivered.");

        var file = _files.FirstOrDefault(f => f.Id == fileId)
                   ?? throw new InvalidOperationException("File not found in order.");

        file.RequestRevision(comment);

        Status = OrderStatus.RevisionRequested;

        AddEvent(new TranslationRevisionRequested(Id, fileId, comment, DateTime.UtcNow));
    }

    public void RespondToRevision(Guid translatorId, Guid fileId, string newTranslatedFilePath)
    {
        if (Status != OrderStatus.RevisionRequested)
            throw new InvalidOperationException("Order is not in revision state.");

        if (_acceptedOffer?.TranslatorId != translatorId)
            throw new InvalidOperationException("Only the assigned translator can respond to revisions.");

        var file = _files.FirstOrDefault(f => f.Id == fileId)
                   ?? throw new InvalidOperationException("File not found.");

        if (file.ReviewStatus != ReviewStatus.RevisionRequested)
            throw new InvalidOperationException("Revision was not requested for this file.");

        file.DeliverTranslatedFile(newTranslatedFilePath);
        file.ClearRevision();

        AddEvent(new TranslationRevisionDelivered(Id, file.Id, newTranslatedFilePath, DateTime.UtcNow));

        if (_files.All(f => f.ReviewStatus == ReviewStatus.None))
        {
            Status = OrderStatus.Delivered; // wracamy z "RevisionRequested" do "Delivered"
        }
    }

    public void CompleteOrder()
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Only delivered orders can be completed.");

        if (_files.Any(f => f.ReviewStatus != ReviewStatus.Approved))
            throw new InvalidOperationException("All files must be approved to complete the order.");

        Status = OrderStatus.Completed;

        AddEvent(new TranslationOrderCompleted(Id, DateTime.UtcNow));
    }
}