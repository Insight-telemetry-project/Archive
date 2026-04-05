using Analyzer_Service.Models.Schema;
using Archive.Models.Schema;

namespace Archive.Models.Dto
{
    public class FlightSuspiciousPointsDto
    {
        public Dictionary<string, List<AnomalyWindow>> Anomalies { get; set; }

        public Dictionary<string, List<HistoricalSimilarityPoint>> HistoricalSimilarity { get; set; }
    }
}
