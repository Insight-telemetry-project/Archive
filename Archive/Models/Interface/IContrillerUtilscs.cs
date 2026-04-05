using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Schema;

namespace Archive.Models.Interface
{
    public interface IContrillerUtilscs
    {
        Task<List<existingFlight>> GetExistingFlights();
        Task<List<string>> GetConnectionsByParameter(int masterIndex, string parameter);
        Task<List<AnomalyWindow>> GetAnomaliesByParameter(int masterIndex, string parameter);
        Task<List<HistoricalSimilarityPoint>> GetHistoricalSimilarityFlightDataAsync(int masterIndex, string parameter);
        Task<FlightSuspiciousPointsDto> GetAllSuspiciousPointsForFlightAsync(int masterIndex);

    }
}
