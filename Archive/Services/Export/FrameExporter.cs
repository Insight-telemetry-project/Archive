using Archive.Models.Dto;
using Archive.Models.Schema;
using System.IO.Compression;

namespace Archive.Services.Export
{
    public abstract class FrameExporter
    {
        protected ZipArchive CreateArchive(MemoryStream zipStream)
        {
            return new ZipArchive(zipStream, ZipArchiveMode.Create, true);
        }

        public abstract Task StartAsync(
            ZipArchive archive,
            int flightId,
            FlightSuspiciousPointsDto specialPoints,
            Dictionary<string, List<string>> connections,
            Dictionary<string, int> fields
        );

        public abstract Task WriteFrameAsync(TelemetrySensorFields frame);

        public abstract Task EndAsync();
    }
}
