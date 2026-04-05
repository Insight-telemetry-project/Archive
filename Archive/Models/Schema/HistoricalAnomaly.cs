using Archive.Models.Dto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Models.Schema
{
    [BsonIgnoreExtraElements]
    public class HistoricalAnomaly
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("MasterIndex")]
        public int MasterIndex { get; set; }

        [BsonElement("ParameterName")]
        public string ParameterName { get; set; }

        [BsonElement("StartEpoch")]
        public long StartEpoch { get; set; }

        [BsonElement("EndEpoch")]
        public long EndEpoch { get; set; }

        [BsonElement("Label")]
        public string Label { get; set; }

        [BsonElement("PatternHash")]
        public string PatternHash { get; set; }

        [BsonElement("FeatureValues")]
        public SegmentFeatures FeatureValues { get; set; } = new SegmentFeatures();
        
        [BsonElement("AnomalyEpochSeconds")]
        public long AnomalyEpochSeconds { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
