using LinguaPoint.Shared.Commands;

namespace LinguaPoint.Shared.Queries;

public interface IQueryHandler<in TQuery, TData> where TQuery : class, IQuery<TData>
{
    Task<Result<TData>> Handle(TQuery query, CancellationToken cancellationToken = default);
}