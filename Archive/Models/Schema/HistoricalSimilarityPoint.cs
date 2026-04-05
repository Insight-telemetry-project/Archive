using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Analyzer_Service.Models.Schema
{
    public class HistoricalSimilarityPoint
    {
        [BsonElement("RecordId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecordId { get; set; } = string.Empty;

        [BsonElement("ComparedFlightIndex")]
        public int ComparedFlightIndex { get; set; }

        [BsonElement("StartEpoch")]
        public long StartEpoch { get; set; }

        [BsonElement("EndEpoch")]
        public long EndEpoch { get; set; }

        [BsonElement("AnomalyTime")]
        public long AnomalyTime { get; set; }

        [BsonElement("Label")]
        public string Label { get; set; } = string.Empty;

        [BsonElement("FinalScore")]
        public double FinalScore { get; set; }
    }
}
