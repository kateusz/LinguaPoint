namespace LinguaPoint.Orders.Domain.Orders;

public enum OrderStatus
{
    New,
    AvailableForBidding,
    OfferAccepted,
    InProgress,
    Delivered,
    Completed,
    RevisionRequested
}
