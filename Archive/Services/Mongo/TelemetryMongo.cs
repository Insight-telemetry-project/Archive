using Archive.Models.Configuration;
using Archive.Models.Constant;
using Archive.Models.Schema;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Services.Mongo
{
    public class TelemetryMongo
    {
        private readonly IMongoCollection<TelemetryFieldsRecord> _telemetryFields;
        private readonly IMongoCollection<TelemetryFlightRecord> _telemetryFlightData;

        public TelemetryMongo(IOptions<MongoSettings> settings)
        {
            MongoSettings mongoSettings = settings.Value;

            MongoClient client = new MongoClient(mongoSettings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoSettings.DatabaseName);

            _telemetryFields = database.GetCollection<TelemetryFieldsRecord>(mongoSettings.CollectionTelemetryFields);
            _telemetryFlightData = database.GetCollection<TelemetryFlightRecord>(mongoSettings.CollectionTelemetryFlightData);
        }

        public async Task<List<TelemetryFieldsRecord>> GetFromFieldsAsync(int masterIndex)
        {
            FilterDefinition<TelemetryFieldsRecord> filter =
                Builders<TelemetryFieldsRecord>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);

            List<TelemetryFieldsRecord> results = await _telemetryFields.Find(filter).ToListAsync();
            return results;
        }

        public async Task<List<TelemetryFlightRecord>> GetFromFlightDataAsync(int masterIndex)
        {
            FilterDefinition<TelemetryFlightRecord> filter =
                Builders<TelemetryFlightRecord>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);

            List<TelemetryFlightRecord> results = await _telemetryFlightData.Find(filter).ToListAsync();
            return results;
        }
    }
}
