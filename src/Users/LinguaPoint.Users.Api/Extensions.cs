using LinguaPoint.Shared.Commands;
using LinguaPoint.Users.Core.Commands;
using LinguaPoint.Users.Core.DTO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LinguaPoint.Users.Api;

public static class Extensions
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            pattern: "users",
            handler: RegisterUser);
    }

    private static async Task<IResult> RegisterUser([FromServices] ICommandHandler<RegisterUserCommand, UserDto> handler,
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        await handler.Handle(command, cancellationToken);
        return Results.StatusCode(StatusCodes.Status201Created);
    }
}