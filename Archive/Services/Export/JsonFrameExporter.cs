using Archive.Models.Dto;
using Archive.Models.Interface.Export;
using Archive.Models.Schema;
using System.IO.Compression;

namespace Archive.Services.Export
{
    public class JsonFrameExporter : IFrameExporter
    {
        private System.Text.Json.Utf8JsonWriter _writer;

        public Task StartAsync(
            ZipArchive archive,
            int flightId,
            FlightSuspiciousPointsDto specialPoints,
            Dictionary<string, List<string>> connections,
            Dictionary<string, int> fields)
        {
            ZipArchiveEntry entry = archive.CreateEntry($"flight_{flightId}.json");

            Stream outputStream = entry.Open();

            _writer = new System.Text.Json.Utf8JsonWriter(
                outputStream,
                new System.Text.Json.JsonWriterOptions
                {
                    Indented = true
                });

            _writer.WriteStartObject();

            _writer.WriteNumber("flightId", flightId);

            _writer.WritePropertyName("anomalies");

            System.Text.Json.JsonSerializer.Serialize(
                _writer,
                specialPoints.Anomalies
            );

            _writer.WritePropertyName("historicalSimilarity");

            System.Text.Json.JsonSerializer.Serialize(
                _writer,
                specialPoints.HistoricalSimilarity
            );

            _writer.WritePropertyName("connections");

            System.Text.Json.JsonSerializer.Serialize(
                _writer,
                connections
            );

            _writer.WriteStartArray("frames");

            return Task.CompletedTask;
        }

        public Task WriteFrameAsync(TelemetrySensorFields frame)
        {
            _writer.WriteStartObject();

            DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(frame.Timestep);
            _writer.WriteString("timestamp", date.UtcDateTime.ToString("o"));

            _writer.WritePropertyName("values");

            System.Text.Json.JsonSerializer.Serialize(_writer, frame.Fields);

            _writer.WriteEndObject();

            return Task.CompletedTask;
        }

        public Task EndAsync()
        {
            _writer.WriteEndArray();
            _writer.WriteEndObject();

            _writer.Flush();

            return Task.CompletedTask;
        }
    }
}
