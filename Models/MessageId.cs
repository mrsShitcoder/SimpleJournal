namespace Journal.Models;

public class MessageId : IComparable<MessageId>
{
    public ulong UserId { get; set; }
    public ulong Sequence { get; set; }

    public MessageId(ulong userId, ulong sequence)
    {
        UserId = userId;
        Sequence = sequence;
    }
    public int CompareTo(MessageId? other)
    {
        if (other == null)
        {
            return 1;
        }
            
        var compareUsers = this.UserId.CompareTo(other.UserId);
        if (compareUsers != 0) return compareUsers;
        return this.Sequence.CompareTo(other.Sequence);
    }
}