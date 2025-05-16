using LinguaPoint.Orders.Application.DTO;
using LinguaPoint.Orders.Domain.Repositories;
using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Handler for publishing a translation order making it available for translators to bid on
/// </summary>
internal class PublishOrderHandler : ICommandHandler<PublishOrderCommand, TranslationOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<PublishOrderHandler> _logger;

    public PublishOrderHandler(
        IOrderRepository orderRepository,
        ILogger<PublishOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result<TranslationOrderDto>> Handle(PublishOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Publishing order {OrderId} for bidding", command.OrderId);

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
                _logger.LogWarning("Client {ClientId} attempted to publish order {OrderId} they don't own",
                    command.ClientId, command.OrderId);
                return Result<TranslationOrderDto>.Failure("You do not have permission to publish this order");
            }

            try
            {
                // Attempt to publish the order
                order.PublishOrder();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when publishing order {OrderId}", command.OrderId);
                return Result<TranslationOrderDto>.Failure(ex.Message);
            }

            // Update the order in the repository
            await _orderRepository.Update(order, cancellationToken);

            // Map to DTO for response
            var orderDto = new TranslationOrderDto
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
                    IsDelivered = f.IsDelivered,
                    ReviewStatus = f.ReviewStatus.ToString()
                }).ToList()
            };

            _logger.LogInformation("Successfully published order {OrderId}", command.OrderId);

            return Result<TranslationOrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing order {OrderId}", command.OrderId);
            return Result<TranslationOrderDto>.Failure($"Failed to publish order: {ex.Message}");
        }
    }
}