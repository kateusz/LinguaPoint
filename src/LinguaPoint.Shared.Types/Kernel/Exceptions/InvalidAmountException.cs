namespace LinguaPoint.Shared.Types.Kernel.Exceptions;

public class InvalidAmountException : DomainException
{
    public decimal Amount { get; }

    public InvalidAmountException(decimal amount) : base($"Amount: '{amount}' is invalid.")
    {
        Amount = amount;
    }
}