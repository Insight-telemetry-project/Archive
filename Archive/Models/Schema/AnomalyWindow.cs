using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Models.Schema
{
    public class AnomalyWindow
    {
        [BsonElement("StartEpoch")]
        public long StartEpoch { get; set; }

        [BsonElement("EndEpoch")]
        public long EndEpoch { get; set; }

        [BsonElement("RepresentativeEpoch")]
        public long RepresentativeEpoch { get; set; }

        [BsonElement("Label")]
        public string Label { get; set; } = string.Empty;
    }
}
