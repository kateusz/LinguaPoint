namespace LinguaPoint.Shared.Types.Kernel.Exceptions;

public class InvalidEmailException : DomainException
{
    public string Email { get; }

    public InvalidEmailException(string email) : base($"Email: '{email}' is invalid.")
    {
        Email = email;
    }
}