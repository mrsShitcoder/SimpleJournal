namespace Journal.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

public class NullToEmptyDocumentSerializer : SerializerBase<BsonDocument>
{
    private readonly BsonDocumentSerializer _bsonDocumentSerializer = new BsonDocumentSerializer();

    public override BsonDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return new BsonDocument();
        }

        return _bsonDocumentSerializer.Deserialize(context, args);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BsonDocument value)
    {
        _bsonDocumentSerializer.Serialize(context, args, value);
    }
}
