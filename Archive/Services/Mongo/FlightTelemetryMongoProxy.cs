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
        private readonly IMongoCollection<HistoricalAnomaly> _historicalAnomalies;
        public FlightTelemetryMongoProxy(IOptions<MongoSettings> settings)
        {
            MongoSettings mongoSettings = settings.Value;

            MongoClient client = new MongoClient(mongoSettings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoSettings.DatabaseName);

            _telemetryFields = database.GetCollection<TelemetrySensorFields>(mongoSettings.CollectionTelemetryFields);
            _telemetryFlightData = database.GetCollection<TelemetryFlightData>(mongoSettings.CollectionTelemetryFlightData);
            _historicalAnomalies = database.GetCollection<HistoricalAnomaly>(mongoSettings.CollectionHistoricalAnomalies);
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
                return null;

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
                return null;

            return results;
        }

        public async Task<List<TelemetryFlightData>> GetAllFlightDataAsync()
        {
            List<TelemetryFlightData> results = await _telemetryFlightData
                .Find(_ => true)
                .Project<TelemetryFlightData>(Builders<TelemetryFlightData>.Projection.Exclude(ConstantFligth.MONGO_ID))
                .ToListAsync();
            if (results.Count == 0)
                return null;
            return results;
        }

        public async Task DeleteAllDataByMasterIndexAsync(int masterIndex)
        {
            FilterDefinition<TelemetrySensorFields> filter = Builders<TelemetrySensorFields>.Filter.Eq("MasterIndex", masterIndex);
            FilterDefinition<TelemetryFlightData> filterFlight = Builders<TelemetryFlightData>.Filter.Eq("MasterIndex", masterIndex);
            FilterDefinition<HistoricalAnomaly> filterAnomalies = Builders<HistoricalAnomaly>.Filter.Eq("MasterIndex", masterIndex);

            await Task.WhenAll(
                _telemetryFields.DeleteManyAsync(filter),
                _telemetryFlightData.DeleteManyAsync(filterFlight),
                _historicalAnomalies.DeleteManyAsync(filterAnomalies)
            );
        }
    }
}
