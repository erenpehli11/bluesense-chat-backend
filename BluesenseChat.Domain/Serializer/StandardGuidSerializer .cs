using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

public class StandardGuidSerializer : SerializerBase<Guid>
{
    private static readonly GuidSerializer _inner = new GuidSerializer(GuidRepresentation.Standard);

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
    {
        _inner.Serialize(context, args, value);
    }

    public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return _inner.Deserialize(context, args);
    }
}
