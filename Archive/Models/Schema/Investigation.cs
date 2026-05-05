using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Models.Schema
{
    [BsonIgnoreExtraElements]
    public class Investigation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("MasterIndex")]
        public int MasterIndex { get; set; }

        [BsonElement("Param")]
        public string Param { get; set; } = string.Empty;

        [BsonElement("Time")]
        public long Time { get; set; }

        [BsonElement("Value")]
        public double Value { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
