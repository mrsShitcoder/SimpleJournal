namespace Journal.Models;

public class JournalDatabaseSettings
{
    public string Connection { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    
    public string MessageContentsCollection { get; set; } = null!;

    public string MessagePreviewsCollection { get; set; } = null!;

    public string UserSequenceCollection { get; set; } = null!;
}