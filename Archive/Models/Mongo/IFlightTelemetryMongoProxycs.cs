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

        Task<List<long>> GetAnomaliesByParameter(int masterIndex, string parameter);

        Task<FlightSuspiciousPointsDto> GetAllSpecialPointsForFlightAsync(int masterIndex);
        Task<IAsyncCursor<TelemetrySensorFields>> GetFromFieldsCursorAsync(int masterIndex);

    }
}
