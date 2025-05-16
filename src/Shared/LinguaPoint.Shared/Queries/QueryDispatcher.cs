using LinguaPoint.Shared.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Shared.Queries;

internal sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<Result<TData>> QueryAsync<TData>(IQuery<TData> query, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TData));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TData>, TData>.Handle));
        if (method is null)
        {
            throw new InvalidOperationException($"Query handler for '{typeof(TData).Name}' is invalid.");
        }

        // ReSharper disable once PossibleNullReferenceException
        return await (Task<Result<TData>>)method.Invoke(handler, new object[] {query, cancellationToken});
    }
}