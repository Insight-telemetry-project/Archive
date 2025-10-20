using Archive.Models.Configuration;
using Archive.Models.Constant;
using Archive.Models.Schema;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Services.Mongo
{
    public class FlightTelemetryMongoProxy
    {
        private readonly IMongoCollection<TelemetrySensorFields> _telemetryFields;
        private readonly IMongoCollection<TelemetryFlightData> _telemetryFlightData;

        public FlightTelemetryMongoProxy(IOptions<MongoSettings> settings)
        {
            MongoSettings mongoSettings = settings.Value;

            MongoClient client = new MongoClient(mongoSettings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoSettings.DatabaseName);

            _telemetryFields = database.GetCollection<TelemetrySensorFields>(mongoSettings.CollectionTelemetryFields);
            _telemetryFlightData = database.GetCollection<TelemetryFlightData>(mongoSettings.CollectionTelemetryFlightData);
        }

        public async Task<List<TelemetrySensorFields>> GetFromFieldsAsync(int masterIndex)
        {
            FilterDefinition<TelemetrySensorFields> filter =
                Builders<TelemetrySensorFields>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);

            List<TelemetrySensorFields> results = await _telemetryFields
                .Find(filter)
                .Project<TelemetrySensorFields>(Builders<TelemetrySensorFields>.Projection.Exclude(ConstantFligth.MONGO_ID))
                .ToListAsync();

            if (results.Count == 0)
                throw new KeyNotFoundException($"No TelemetryFields found for Master Index {masterIndex}");

            return results;
        }

        public async Task<List<TelemetryFlightData>> GetFromFlightDataAsync(int masterIndex)
        {
            FilterDefinition<TelemetryFlightData> filter =
                Builders<TelemetryFlightData>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);

            List<TelemetryFlightData> results = await _telemetryFlightData
                .Find(filter)
                .Project<TelemetryFlightData>(Builders<TelemetryFlightData>.Projection.Exclude(ConstantFligth.MONGO_ID))
                .ToListAsync();

            if (results.Count == 0)
                throw new KeyNotFoundException($"No TelemetryFlightData found for Master Index {masterIndex}");

            return results;
        }

    }
}
