namespace LinguaPoint.Shared.Types.Kernel.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}