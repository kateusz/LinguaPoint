namespace LinguaPoint.Shared.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : class, ICommand
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
}