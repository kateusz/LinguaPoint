namespace LinguaPoint.Shared.Types.Kernel.ValueObjects;

public class FilePath : IEquatable<FilePath>
{
    public string Value { get; }

    public FilePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be empty.", nameof(path));

        if (Path.GetInvalidPathChars().Length > 0 && path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            throw new ArgumentException("File path contains invalid characters.");

        Value = path;
    }

    public override string ToString() => Value;

    public bool Equals(FilePath? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as FilePath);

    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

    public static implicit operator string(FilePath path) => path.Value;
}