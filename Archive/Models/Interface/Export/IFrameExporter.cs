using Archive.Models.Dto;
using Archive.Models.Schema;
using System.IO.Compression;

namespace Archive.Models.Interface.Export
{
    public interface IFrameExporter
    {
        Task StartAsync(
            ZipArchive archive,
            int flightId,
            FlightSuspiciousPointsDto specialPoints,
            Dictionary<string, List<string>> connections,
            Dictionary<string, int> fields
         );

        Task WriteFrameAsync(TelemetrySensorFields frame);

        Task EndAsync();
    }
}
