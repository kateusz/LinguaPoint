namespace LinguaPoint.Shared.Commands;

public interface ICommandDispatcher
{
    Task<Result> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand;
    
    Task<Result<TResult>> Dispatch<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand;
}