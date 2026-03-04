using Archive.Models.Dto;
using Archive.Models.Enum;
using Archive.Models.Interface.Export;
using Archive.Models.Mongo;
using Archive.Models.Schema;
using Archive.Services.Mongo;
using MongoDB.Driver;
using System.IO;
using System.IO.Compression;
namespace Archive.Services.Export
{
    public class ExportService : IExportService
    {
        private readonly IFlightTelemetryMongoProxy _flightTelemetryMongoProxy;

        public ExportService(IFlightTelemetryMongoProxy flightTelemetryMongoProxy)
        {
            _flightTelemetryMongoProxy = flightTelemetryMongoProxy;
        }



        public async Task<Stream> ExportFlightAsync(int masterIndex, ExportFormat format)
        {
            IFrameExporter exporter = CreateExporter(format);

            MemoryStream zipStream = new MemoryStream();

            ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

            FlightSuspiciousPointsDto specialPoints =
                await _flightTelemetryMongoProxy.GetAllSpecialPointsForFlightAsync(masterIndex);

            TelemetryFlightData flightData =
                await _flightTelemetryMongoProxy.GetFromFlightDataAsync(masterIndex);

            await exporter.StartAsync(
                archive,
                masterIndex,
                specialPoints,
                flightData.Connections,
                flightData.Fields
            );

            IAsyncCursor<TelemetrySensorFields> cursor =
                await _flightTelemetryMongoProxy.GetFromFieldsCursorAsync(masterIndex);

            while (await cursor.MoveNextAsync())
            {
                foreach (TelemetrySensorFields frame in cursor.Current)
                {
                    await exporter.WriteFrameAsync(frame);
                }
            }

            await exporter.EndAsync();

            archive.Dispose();

            zipStream.Position = 0;

            return zipStream;
        }

        private IFrameExporter CreateExporter(ExportFormat format)
        {
            if (format == ExportFormat.Json)
            {
                return new JsonFrameExporter();
            }

            if (format == ExportFormat.Csv)
            {
                return new CsvFrameExporter();
            }

            if (format == ExportFormat.Pcap)
            {
                return new PcapFrameExporter();
            }

            throw new Exception("Unsupported export format");
        }
    }
}
