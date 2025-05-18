using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LinguaPoint.Marketplace.Api;

public static class Extensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            pattern: "",
            handler: null);
    }
}