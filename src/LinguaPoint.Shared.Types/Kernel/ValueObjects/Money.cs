namespace LinguaPoint.Shared.Types.Kernel.ValueObjects;

public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public override string ToString() => $"{Amount} {Currency}";
}