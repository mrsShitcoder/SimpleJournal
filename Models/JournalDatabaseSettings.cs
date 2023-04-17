namespace Journal.Models;

public class JournalDatabaseSettings
{
    public string Connection { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string MessagesCollection { get; set; } = null!;
    public string CurrentMessageIdCollection { get; set; } = null!;
}