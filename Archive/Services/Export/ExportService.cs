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
        private readonly IFactoryFormat _frameExporterFactory;

        public ExportService(
            IFlightTelemetryMongoProxy flightTelemetryMongoProxy,
            IFactoryFormat frameExporterFactory)
        {
            _flightTelemetryMongoProxy = flightTelemetryMongoProxy;
            _frameExporterFactory = frameExporterFactory;
        }

        public async Task<Stream> ExportFlightAsync(int masterIndex, ExportFormat format)
        {
            IFrameExporter exporter = _frameExporterFactory.CreateExporter(format);

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
    }
}
