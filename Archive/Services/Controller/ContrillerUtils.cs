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
    }
}
