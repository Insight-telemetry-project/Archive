using Archive.Models.Dto;
using Archive.Models.Schema;

namespace Archive.Models.Interface
{
    public interface IContrillerUtilscs
    {
        Task<List<existingFlight>> GetExistingFlights();
    }
}
