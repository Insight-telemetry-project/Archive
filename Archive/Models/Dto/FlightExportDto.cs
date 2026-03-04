using Analyzer_Service.Models.Schema;
using Archive.Models.Schema;

namespace Archive.Models.Dto
{
    public class FlightExportDto
    {
        public int FlightId { get; set; }

        public List<TelemetrySensorFields> Frames { get; set; }

        public Dictionary<string, List<long>> Anomalies { get; set; }

        public Dictionary<string, List<HistoricalSimilarityPoint>> HistoricalSimilarity { get; set; }

        public Dictionary<string, List<string>> Connections { get; set; }
    }
}
