using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

public class NullableStandardGuidSerializer : SerializerBase<Guid?>
{
    private static readonly GuidSerializer _inner = new GuidSerializer(GuidRepresentation.Standard);

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid? value)
    {
        if (value.HasValue)
        {
            _inner.Serialize(context, args, value.Value);
        }
        else
        {
            context.Writer.WriteNull();
        }
    }

    public override Guid? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }

        return _inner.Deserialize(context, args);
    }
}
