namespace Journal.Models;
using MongoDB.Bson.Serialization.Attributes;


public class UserSequence
{
    [BsonId] 
    public ulong UserId { get; set; }
    
    public ulong MaxSequence { get; set; }
}
