using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Interface;
using Archive.Models.Schema;
using Archive.Services.Mongo;
using System.Threading.Tasks;

namespace Archive.Services.Controller
{
    public class ContrillerUtils : IContrillerUtilscs
    {
        private readonly FlightTelemetryMongoProxy _telemetryMongoProxy;
        public ContrillerUtils(FlightTelemetryMongoProxy flightTelemetryMongoProxy)
        {
            _telemetryMongoProxy = flightTelemetryMongoProxy;
        }
        public async Task<List<existingFlight>> GetExistingFlights()
        {
            List<TelemetryFlightData> flightDataList = await _telemetryMongoProxy.GetAllFlightDataAsync();
            List<existingFlight> existingFlights = new List<existingFlight>();
            foreach (TelemetryFlightData flightData in flightDataList)
            {
                int flightNumber = flightData.MasterIndex;
                int flightLength = flightData.Fields["flight_length"];
                existingFlight flight = new existingFlight(flightNumber, flightLength);
                existingFlights.Add(flight);
            }
            return existingFlights;
        }

        public async Task<List<HistoricalSimilarityPoint>> GetHistoricalSimilarityFlightDataAsync(int masterIndex, string parameter)
        {
            return await _telemetryMongoProxy.GetHistoricalSimilarityByParamter(masterIndex, parameter);
        }

        public async Task<List<long>> GetAnomaliesByParameter(int masterIndex, string parameter)
        {
            return await _telemetryMongoProxy.GetAnomaliesByParameter(masterIndex, parameter);
        }

        public async Task<List<string>> GetConnectionsByParameter(int masterIndex, string parameter)
        {
            return await _telemetryMongoProxy.GetConnectionsByParameter(masterIndex, parameter);
        }

        public async Task<FlightSuspiciousPointsDto> GetAllSuspiciousPointsForFlightAsync(int masterIndex)
        {
            return await _telemetryMongoProxy.GetAllSpecialPointsForFlightAsync(masterIndex);
        }
    }
}
