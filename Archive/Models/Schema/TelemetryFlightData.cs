using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Models.Schema
{
    public class TelemetryFlightData
    {
        [BsonElement("Master Index")]
        public int MasterIndex { get; set; }

        [BsonElement("Fields")]
        public Dictionary<string, int> Fields { get; set; } = new();
        
        [BsonElement("Connections")]
        public Dictionary<string, List<string>> Connections { get; set; } = new();
    }
}
