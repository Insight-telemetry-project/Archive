using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Schema;
using MongoDB.Driver;

namespace Archive.Models.Mongo
{
    public interface IFlightTelemetryMongoProxy
    {
        Task<List<TelemetrySensorFields>> GetFromFieldsAsync(int masterIndex);

        Task<TelemetryFlightData> GetFromFlightDataAsync(int masterIndex);

        Task<List<HistoricalAnomaly>> GetFromHistoricalAnomaliesAsync(int masterIndex);

        Task<List<TelemetryFlightData>> GetAllFlightDataAsync();

        Task DeleteAllDataByMasterIndexAsync(int masterIndex);

        Task<List<HistoricalSimilarityPoint>> GetHistoricalSimilarityByParamter(int masterIndex, string parameter);

        Task<List<string>> GetConnectionsByParameter(int masterIndex, string parameter);

        Task<List<AnomalyWindow>> GetAnomaliesByParameter(int masterIndex, string parameter);

        Task<FlightSuspiciousPointsDto> GetAllSpecialPointsForFlightAsync(int masterIndex);
        Task<IAsyncCursor<TelemetrySensorFields>> GetFromFieldsCursorAsync(int masterIndex);

        Task<Investigation> CreateInvestigationAsync(Investigation investigation);
        Task<List<Investigation>> GetInvestigationsByFlightAsync(int masterIndex);
        Task<Investigation?> UpdateInvestigationAsync(string id, string name, string description);
        Task DeleteInvestigationAsync(string id);

        Task RemoveHistoricalReferencesToFlightAsync(int deletedFlightId);
    }
}
