namespace LinguaPoint.Shared.Commands;

public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task Handle(TCommand command, CancellationToken cancellationToken = default);
}