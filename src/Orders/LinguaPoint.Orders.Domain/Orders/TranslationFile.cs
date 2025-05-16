namespace LinguaPoint.Orders.Domain.Orders;

public class TranslationFile
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public FileStatus FileStatus { get; private set; }
    public string? Error { get; private set; }
    public string? TranslatedFilePath { get; private set; }
    public bool IsDelivered => !string.IsNullOrWhiteSpace(TranslatedFilePath);
    public ReviewStatus ReviewStatus { get; private set; } = ReviewStatus.None;
    public string? RevisionComment { get; private set; }

    internal TranslationFile(Guid id, Guid orderId, string fileName, string filePath)
    {
        Id = id;
        OrderId = orderId;
        FileName = fileName;
        FilePath = filePath;
        FileStatus = FileStatus.Pending;
    }

    public void MarkAsProcessed()
    {
        FileStatus = FileStatus.Processed;
        Error = null;
    }

    public void MarkAsFailed(string reason)
    {
        FileStatus = FileStatus.Error;
        Error = reason;
    }
    
    public void DeliverTranslatedFile(string translatedFilePath)
    {
        if (FileStatus != FileStatus.Processed)
            throw new InvalidOperationException("File must be processed before delivering translation.");

        if (!string.IsNullOrWhiteSpace(TranslatedFilePath))
            throw new InvalidOperationException("Translation already delivered.");

        TranslatedFilePath = translatedFilePath;
    }

    public void Approve()
    {
        if (!IsDelivered)
            throw new InvalidOperationException("Cannot approve an undelivered translation.");

        if (ReviewStatus != ReviewStatus.None)
            throw new InvalidOperationException("Translation already reviewed.");

        ReviewStatus = ReviewStatus.Approved;
    }

    public void RequestRevision(string comment)
    {
        if (!IsDelivered)
            throw new InvalidOperationException("Cannot request revision on undelivered translation.");

        if (ReviewStatus != ReviewStatus.None)
            throw new InvalidOperationException("Translation already reviewed.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Revision comment is required.", nameof(comment));

        ReviewStatus = ReviewStatus.RevisionRequested;
        RevisionComment = comment;
    }
    
    public void ClearRevision()
    {
        ReviewStatus = ReviewStatus.None;
        RevisionComment = null;
    }

}