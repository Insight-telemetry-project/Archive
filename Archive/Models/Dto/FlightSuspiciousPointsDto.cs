using Analyzer_Service.Models.Schema;

namespace Archive.Models.Dto
{
    public class FlightSuspiciousPointsDto
    {
        public Dictionary<string, List<long>> Anomalies { get; set; }
        public Dictionary<string, List<HistoricalSimilarityPoint>> HistoricalSimilarity { get; set; }
    }
}
