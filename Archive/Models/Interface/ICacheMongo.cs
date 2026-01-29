using Archive.Models.Schema;

namespace Archive.Models.Interface
{
    public interface ICacheMongo
    {
        Task<List<long>> GetParamterFlightDataAsync(int masterIndex, string parameter);
        Task<List<string>> GetConnectionsFlightDataAsync(int masterIndex, string parameter);

    }
}
