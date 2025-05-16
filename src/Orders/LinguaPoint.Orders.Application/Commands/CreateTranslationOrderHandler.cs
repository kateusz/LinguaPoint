using LinguaPoint.Orders.Application.DTO;
using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Orders.Domain.Repositories;
using LinguaPoint.Shared;
using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Types.Kernel;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Orders.Application.Commands;

/// <summary>
/// Handler for creating a new translation order
/// </summary>
internal class CreateTranslationOrderHandler : ICommandHandler<CreateTranslationOrderCommand, TranslationOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreateTranslationOrderHandler> _logger;

    public CreateTranslationOrderHandler(
        IOrderRepository orderRepository,
        IDomainEventDispatcher eventDispatcher,
        ILogger<CreateTranslationOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<TranslationOrderDto>> Handle(CreateTranslationOrderCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new translation order for client: {ClientId}", command.ClientId);

            // Create the language pair value object
            var languagePair = new LanguagePair(command.SourceLanguage, command.TargetLanguage);
            
            // Create the domain entity using the factory method
            var order = TranslationOrder.Create(command.ClientId, languagePair);
            
            // If an initial file was provided, add it to the order
            if (!string.IsNullOrEmpty(command.InitialFilePath) && !string.IsNullOrEmpty(command.InitialFileName))
            {
                order.AddFile(command.InitialFilePath, command.InitialFileName);
                _logger.LogInformation("Added initial file to order: {FileName}", command.InitialFileName);
            }
            
            // Persist the new order
            await _orderRepository.Add(order, cancellationToken);
            
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
                    Status = f.FileStatus.ToString()
                }).ToList()
            };
            
            _logger.LogInformation("Successfully created translation order with ID: {OrderId}", order.Id);
            
            return Result<TranslationOrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating translation order for client {ClientId}", command.ClientId);
            return Result<TranslationOrderDto>.Failure($"Failed to create translation order: {ex.Message}");
        }
    }
}