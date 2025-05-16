using LinguaPoint.Orders.Application.DTO;
using LinguaPoint.Orders.Domain.Repositories;
using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Orders.Application.Commands;

internal class AddFileToOrderHandler : ICommandHandler<AddFileToOrderCommand, TranslationFileDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<AddFileToOrderHandler> _logger;

    public AddFileToOrderHandler(
        IOrderRepository orderRepository,
        IDomainEventDispatcher eventDispatcher,
        ILogger<AddFileToOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<TranslationFileDto>> Handle(AddFileToOrderCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding file to order {OrderId}: {FileName}", command.OrderId, command.FileName);

            // Retrieve the order from the repository
            var order = await _orderRepository.GetById(new AggregateId(command.OrderId), cancellationToken);
            
            // Verify order exists
            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", command.OrderId);
                return Result<TranslationFileDto>.Failure($"Order with ID {command.OrderId} not found");
            }
            
            // Verify client owns this order
            if (order.ClientId != command.ClientId)
            {
                _logger.LogWarning("Client {ClientId} attempted to add file to order {OrderId} they don't own", 
                    command.ClientId, command.OrderId);
                return Result<TranslationFileDto>.Failure("You do not have permission to add files to this order");
            }
            
            // Add the file to the order
            order.AddFile(command.FilePath, command.FileName);
            
            // Update the order in the repository
            await _orderRepository.Update(order, cancellationToken);
            
            // Get the newly added file (it should be the last one)
            var addedFile = order.Files.LastOrDefault();
            
            if (addedFile == null)
            {
                return Result<TranslationFileDto>.Failure("Failed to add file to order");
            }
            
            // Map to DTO for response
            var fileDto = new TranslationFileDto
            {
                Id = addedFile.Id,
                FileName = addedFile.FileName,
                Status = addedFile.FileStatus.ToString(),
                IsDelivered = addedFile.IsDelivered,
                ReviewStatus = addedFile.ReviewStatus.ToString()
            };
            
            _logger.LogInformation("Successfully added file {FileId} to order {OrderId}", fileDto.Id, command.OrderId);
            
            return Result<TranslationFileDto>.Success(fileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding file to order {OrderId}", command.OrderId);
            return Result<TranslationFileDto>.Failure($"Failed to add file to order: {ex.Message}");
        }
    }
}