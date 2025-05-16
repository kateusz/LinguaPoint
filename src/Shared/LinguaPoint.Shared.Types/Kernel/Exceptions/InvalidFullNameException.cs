namespace LinguaPoint.Shared.Types.Kernel.Exceptions;

public class InvalidFullNameException : DomainException
{
    public string FullName { get; }

    public InvalidFullNameException(string fullName) : base($"Full name: '{fullName}' is invalid.")
    {
        FullName = fullName;
    }
}