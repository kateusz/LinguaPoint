namespace LinguaPoint.Orders.Domain.Orders;

public class LanguagePair : IEquatable<LanguagePair>
{
    public string SourceLanguage { get; }
    public string TargetLanguage { get; }

    public LanguagePair(string sourceLanguage, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(sourceLanguage))
            throw new ArgumentException("Source language is required.", nameof(sourceLanguage));

        if (string.IsNullOrWhiteSpace(targetLanguage))
            throw new ArgumentException("Target language is required.", nameof(targetLanguage));

        if (string.Equals(sourceLanguage, targetLanguage, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Source and target languages must differ.");

        SourceLanguage = sourceLanguage.ToLowerInvariant();
        TargetLanguage = targetLanguage.ToLowerInvariant();
    }

    public override string ToString() => $"{SourceLanguage}-{TargetLanguage}";

    public bool Equals(LanguagePair? other)
    {
        if (other is null) return false;
        return SourceLanguage == other.SourceLanguage && TargetLanguage == other.TargetLanguage;
    }

    public override bool Equals(object? obj) => Equals(obj as LanguagePair);

    public override int GetHashCode() => HashCode.Combine(SourceLanguage, TargetLanguage);
}