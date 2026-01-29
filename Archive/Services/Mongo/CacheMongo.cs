using Archive.Models.Interface;
using Archive.Models.Schema;
using Microsoft.Extensions.Caching.Memory;

namespace Archive.Services.Mongo
{
    public class CacheMongo: ICacheMongo
    {
        private readonly FlightTelemetryMongoProxy _flightTelemetryMongoProxy;
        private readonly IMemoryCache _memoryCache;

        public CacheMongo(FlightTelemetryMongoProxy flightTelemetryMongoProxy, IMemoryCache memoryCache)
        {
            _flightTelemetryMongoProxy = flightTelemetryMongoProxy;
            _memoryCache = memoryCache;
        }

        private async Task<TelemetryFlightData> GetAllFlightDataAsync(int masterIndex)
        {
            string cacheKey = "TelemetryFlightData:MasterIndex:" + masterIndex.ToString();

            TelemetryFlightData cachedResults;
            if (_memoryCache.TryGetValue(cacheKey, out cachedResults))
            {
                return cachedResults;
            }

            TelemetryFlightData results = await _flightTelemetryMongoProxy.GetFromFlightDataAsync(masterIndex);

            

            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _memoryCache.Set(cacheKey, results, cacheOptions);

            return results;
        }
        public async Task<List<long>> GetParamterFlightDataAsync(int masterIndex,string parameter)
        {
            TelemetryFlightData flightData = await GetAllFlightDataAsync(masterIndex);
            return flightData.Anomalies[parameter];
        }

        public async Task<List<string>> GetConnectionsFlightDataAsync(int masterIndex, string parameter)
        {
            TelemetryFlightData flightData = await GetAllFlightDataAsync(masterIndex);
            return flightData.Connections[parameter];
        }

    }
}
