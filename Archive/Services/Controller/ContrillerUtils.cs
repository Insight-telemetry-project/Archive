using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Interface;
using Archive.Models.Mongo;
using Archive.Models.Schema;
using Archive.Services.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Services.Controller
{
    public class ContrillerUtils : IContrillerUtilscs
    {
        private readonly IFlightTelemetryMongoProxy _telemetryMongoProxy;

        public ContrillerUtils(IFlightTelemetryMongoProxy flightTelemetryMongoProxy)
        {
            _telemetryMongoProxy = flightTelemetryMongoProxy;
        }

        public async Task<List<existingFlight>> GetExistingFlights()
        {
            List<TelemetryFlightData> flightDataList =
                await _telemetryMongoProxy.GetAllFlightDataAsync();

            if (flightDataList == null)
            {
                return new List<existingFlight>();
            }

            List<existingFlight> existingFlights = new List<existingFlight>();

            foreach (TelemetryFlightData flightData in flightDataList)
            {
                int flightNumber = flightData.MasterIndex;

                int flightLength = 0;

                if (flightData.Fields != null &&
                    flightData.Fields.TryGetValue("flight_length", out int length))
                {
                    flightLength = length;
                }

                existingFlight flight =new existingFlight(flightNumber, flightLength);

                existingFlights.Add(flight);
            }

            return existingFlights;
        }

        public async Task<List<HistoricalSimilarityPoint>> GetHistoricalSimilarityFlightDataAsync(int masterIndex, string parameter)
        {
            return await _telemetryMongoProxy.GetHistoricalSimilarityByParamter(masterIndex, parameter);
        }

        public async Task<List<AnomalyWindow>> GetAnomaliesByParameter(int masterIndex, string parameter)
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