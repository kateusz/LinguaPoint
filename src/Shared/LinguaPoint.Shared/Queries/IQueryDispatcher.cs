using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Shared.Queries;

public interface IQueryDispatcher
{
    Task<Result<TData>> QueryAsync<TData>(IQuery<TData> query, CancellationToken cancellationToken = default);
}