using LinguaPoint.Orders.Application.Commands;
using LinguaPoint.Orders.Application.DTO;
using LinguaPoint.Shared.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LinguaPoint.Orders.Api;

public static class Extensions
{
    public static IEndpointRouteBuilder RegisterOrderEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(
            pattern: "orders",
            handler: CreateOrder);
            
        return builder;
    }

    private static async Task<IResult> CreateOrder(
        [FromServices] ICommandHandler<CreateTranslationOrderCommand, TranslationOrderDto> handler,
        [FromBody] CreateTranslationOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { error = result.ErrorMessage });
        }

        return Results.Created($"/orders/{result.Data?.Id}", result.Data);
    }
}