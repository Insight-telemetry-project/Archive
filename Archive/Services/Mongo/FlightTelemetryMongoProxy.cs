using Analyzer_Service.Models.Schema;
using Archive.Models.Configuration;
using Archive.Models.Constant;
using Archive.Models.Dto;
using Archive.Models.Mongo;
using Archive.Models.Schema;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Services.Mongo
{
    public class FlightTelemetryMongoProxy : IFlightTelemetryMongoProxy
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
                .SortBy(field => field.Timestep)
                .ToListAsync();

            if (results.Count == 0)
                return null;

            return results;
        }

        public async Task<TelemetryFlightData> GetFromFlightDataAsync(int masterIndex)
        {
            FilterDefinition<TelemetryFlightData> filter =
                Builders<TelemetryFlightData>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);

            TelemetryFlightData result = await _telemetryFlightData
                .Find(filter)
                .Project<TelemetryFlightData>(
                    Builders<TelemetryFlightData>.Projection.Exclude(ConstantFligth.MONGO_ID))
                .FirstOrDefaultAsync();

            return result;
        }
        public async Task<List<HistoricalAnomaly>> GetFromHistoricalAnomaliesAsync(int masterIndex)
        {
            FilterDefinition<HistoricalAnomaly> filter =
                Builders<HistoricalAnomaly>.Filter.Eq(ConstantFligth.FLIGHT_ID, masterIndex);
            List<HistoricalAnomaly> results = await _historicalAnomalies
                .Find(filter)
                .Project<HistoricalAnomaly>(Builders<HistoricalAnomaly>.Projection.Exclude(ConstantFligth.MONGO_ID))
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


        public async Task<List<HistoricalSimilarityPoint>>GetHistoricalSimilarityByParamter(int masterIndex, string parameter)
        {
            FilterDefinition<TelemetryFlightData> filterDefinition =
                Builders<TelemetryFlightData>.Filter.Eq(
                    flightData => flightData.MasterIndex,
                    masterIndex);

            string historicalSimilarityFieldPath =$"HistoricalSimilarity.{parameter}";

            ProjectionDefinition<TelemetryFlightData> projectionDefinition =
                Builders<TelemetryFlightData>.Projection
                    .Include(historicalSimilarityFieldPath);

            BsonDocument projectedDocument =
                await _telemetryFlightData
                    .Find(filterDefinition)
                    .Project<BsonDocument>(projectionDefinition)
                    .FirstAsync();

            BsonDocument historicalSimilarityDocument =
                projectedDocument["HistoricalSimilarity"].AsBsonDocument;

            BsonArray similarityPointsArray =historicalSimilarityDocument[parameter].AsBsonArray;

            List<HistoricalSimilarityPoint> similarityPoints =
                similarityPointsArray.Select(point =>BsonSerializer.Deserialize<HistoricalSimilarityPoint>(point.AsBsonDocument)).ToList();

            return similarityPoints;
        }

        public async Task<List<string>> GetConnectionsByParameter(int masterIndex, string parameter)
        {
            FilterDefinition<TelemetryFlightData> filterDefinition =
                Builders<TelemetryFlightData>.Filter.Eq(
                    flightData => flightData.MasterIndex,
                    masterIndex);

            string connectionsFieldPath = $"Connections.{parameter}";

            ProjectionDefinition<TelemetryFlightData> projectionDefinition =
                Builders<TelemetryFlightData>.Projection
                    .Include(connectionsFieldPath);

            BsonDocument projectedDocument =
                await _telemetryFlightData
                    .Find(filterDefinition)
                    .Project<BsonDocument>(projectionDefinition)
                    .FirstAsync();

            BsonDocument connectionsDocument =
                projectedDocument["Connections"].AsBsonDocument;

            BsonArray connectionsArray =
                connectionsDocument[parameter].AsBsonArray;

            List<string> connections =
                connectionsArray
                    .Select(connectionValue => connectionValue.AsString)
                    .ToList();

            return connections;
        }

        public async Task<List<long>> GetAnomaliesByParameter(int masterIndex, string parameter)
        {
            FilterDefinition<TelemetryFlightData> filterDefinition =
                Builders<TelemetryFlightData>.Filter.Eq(
                    flightData => flightData.MasterIndex,
                    masterIndex);

            string anomaliesFieldPath = $"Anomalies.{parameter}";

            ProjectionDefinition<TelemetryFlightData> projectionDefinition =
                Builders<TelemetryFlightData>.Projection
                    .Include(anomaliesFieldPath);

            BsonDocument projectedDocument =
                await _telemetryFlightData
                    .Find(filterDefinition)
                    .Project<BsonDocument>(projectionDefinition)
                    .FirstAsync();

            BsonDocument anomaliesDocument =
                projectedDocument["Anomalies"].AsBsonDocument;

            BsonArray anomaliesArray =
                anomaliesDocument[parameter].AsBsonArray;

            List<long> anomalies =
                anomaliesArray
                    .Select(anomalyValue => anomalyValue.ToInt64())
                    .ToList();

            return anomalies;
        }

        public async Task<FlightSuspiciousPointsDto> GetAllSpecialPointsForFlightAsync(int masterIndex)
        {
            FilterDefinition<TelemetryFlightData> filterDefinition =
                Builders<TelemetryFlightData>.Filter.Eq(
                    flightData => flightData.MasterIndex,
                    masterIndex);

            ProjectionDefinition<TelemetryFlightData> projectionDefinition =
                Builders<TelemetryFlightData>.Projection
                    .Include("Anomalies")
                    .Include("HistoricalSimilarity")
                    .Exclude("_id");

            BsonDocument projectedDocument =
                await _telemetryFlightData
                    .Find(filterDefinition)
                    .Project<BsonDocument>(projectionDefinition)
                    .FirstOrDefaultAsync();

            if (projectedDocument == null)
                return null;

            FlightSuspiciousPointsDto flightSpecialPointsDto = new FlightSuspiciousPointsDto
            {
                Anomalies = new Dictionary<string, List<long>>(),
                HistoricalSimilarity = new Dictionary<string, List<HistoricalSimilarityPoint>>()
            };

            if (projectedDocument.Contains("Anomalies"))
            {
                BsonDocument anomaliesDocument = projectedDocument["Anomalies"].AsBsonDocument;

                foreach (BsonElement parameterElement in anomaliesDocument.Elements)
                {
                    string parameterName = parameterElement.Name;

                    BsonArray anomaliesArray = parameterElement.Value.AsBsonArray;

                    List<long> anomalyPoints = anomaliesArray
                        .Select(anomalyValue => anomalyValue.ToInt64())
                        .ToList();

                    flightSpecialPointsDto.Anomalies.Add(parameterName, anomalyPoints);
                }
            }

            if (projectedDocument.Contains("HistoricalSimilarity"))
            {
                BsonDocument historicalSimilarityDocument =
                    projectedDocument["HistoricalSimilarity"].AsBsonDocument;

                foreach (BsonElement parameterElement in historicalSimilarityDocument.Elements)
                {
                    string parameterName = parameterElement.Name;

                    BsonArray similarityArray = parameterElement.Value.AsBsonArray;

                    List<HistoricalSimilarityPoint> similarityPoints =
                        similarityArray
                            .Select(point =>
                                BsonSerializer.Deserialize<HistoricalSimilarityPoint>(
                                    point.AsBsonDocument))
                            .ToList();

                    flightSpecialPointsDto.HistoricalSimilarity.Add(parameterName, similarityPoints);
                }
            }

            return flightSpecialPointsDto;
        }


        public async Task<IAsyncCursor<TelemetrySensorFields>> GetFromFieldsCursorAsync(int masterIndex)
        {
            FilterDefinition<TelemetrySensorFields> filter =
                Builders<TelemetrySensorFields>.Filter.Eq(
                    ConstantFligth.FLIGHT_ID,
                    masterIndex);

            FindOptions<TelemetrySensorFields> options =
                new FindOptions<TelemetrySensorFields>
                {
                    Sort = Builders<TelemetrySensorFields>.Sort
                        .Ascending(field => field.Timestep),

                    Projection = Builders<TelemetrySensorFields>.Projection
                        .Exclude(ConstantFligth.MONGO_ID),

                    BatchSize = 500
                };

            IAsyncCursor<TelemetrySensorFields> cursor =
                await _telemetryFields.FindAsync(filter, options);

            return cursor;
        }

    }
}
