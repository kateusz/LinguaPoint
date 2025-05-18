using LinguaPoint.Orders.Application.DTO;
using LinguaPoint.Orders.Domain.Repositories;
using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Handler for accepting a translator's offer on a translation order
/// </summary>
internal class AcceptOfferHandler : ICommandHandler<AcceptOfferCommand, TranslationOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<AcceptOfferHandler> _logger;

    public AcceptOfferHandler(
        IOrderRepository orderRepository,
        IDomainEventDispatcher eventDispatcher,
        ILogger<AcceptOfferHandler> logger)
    {
        _orderRepository = orderRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<TranslationOrderDto>> Handle(AcceptOfferCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Accepting offer from translator {TranslatorId} for order {OrderId}", 
                command.TranslatorId, command.OrderId);

            // Retrieve the order from the repository
            var order = await _orderRepository.GetById(new AggregateId(command.OrderId), cancellationToken);
            
            // Verify order exists
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", command.OrderId);
                return Result<TranslationOrderDto>.Failure($"Order with ID {command.OrderId} not found");
            }
            
            // Verify client owns this order
            if (order.ClientId != command.ClientId)
            {
                _logger.LogWarning("Client {ClientId} attempted to accept offer for order {OrderId} they don't own", 
                    command.ClientId, command.OrderId);
                return Result<TranslationOrderDto>.Failure("You do not have permission to accept offers for this order");
            }
            
            try
            {
                // Attempt to accept the offer
                order.AcceptOffer(command.TranslatorId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when accepting offer for order {OrderId}", command.OrderId);
                return Result<TranslationOrderDto>.Failure(ex.Message);
            }
            
            // Update the order in the repository
            await _orderRepository.Update(order, cancellationToken);
            
            // Dispatch domain events
            foreach (var @event in order.Events)
            {
                await _eventDispatcher.DispatchAsync(@event, cancellationToken);
            }
            
            // Map to DTO for response
            var orderDto = MapToDto(order);
            
            _logger.LogInformation("Successfully accepted offer from translator {TranslatorId} for order {OrderId}", 
                command.TranslatorId, command.OrderId);
            
            return Result<TranslationOrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting offer for order {OrderId}", command.OrderId);
            return Result<TranslationOrderDto>.Failure($"Failed to accept offer: {ex.Message}");
        }
    }

    private static TranslationOrderDto MapToDto(Domain.Orders.TranslationOrder order)
    {
        var dto = new TranslationOrderDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            SourceLanguage = order.LanguagePair.SourceLanguage,
            TargetLanguage = order.LanguagePair.TargetLanguage,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Files = order.Files.Select(f => new TranslationFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                Status = f.FileStatus.ToString(),
                Error = f.Error,
                TranslatedFilePath = f.TranslatedFilePath,
                IsDelivered = f.IsDelivered,
                ReviewStatus = f.ReviewStatus.ToString(),
                RevisionComment = f.RevisionComment
            }).ToList(),
            Offers = order.Offers.Select(o => new TranslationOfferDto
            {
                Id = o.Id,
                TranslatorId = o.TranslatorId,
                Price = o.Price.Amount,
                Currency = o.Price.Currency,
                CreatedAt = o.CreatedAt
            }).ToList()
        };

        // Add accepted offer if available
        if (order.AcceptedOffer != null)
        {
            dto.AcceptedOffer = new TranslationOfferDto
            {
                Id = order.AcceptedOffer.Id,
                TranslatorId = order.AcceptedOffer.TranslatorId,
                Price = order.AcceptedOffer.Price.Amount,
                Currency = order.AcceptedOffer.Price.Currency,
                CreatedAt = order.AcceptedOffer.CreatedAt
            };
            
            // Set order price based on accepted offer
            dto.Amount = order.AcceptedOffer.Price.Amount;
            dto.Currency = order.AcceptedOffer.Price.Currency;
        }

        return dto;
    }
}