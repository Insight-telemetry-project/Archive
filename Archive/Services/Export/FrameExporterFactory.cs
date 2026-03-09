using Archive.Models.Enum;
using Archive.Models.Interface.Export;
using Microsoft.AspNetCore.Rewrite;

namespace Archive.Services.Export
{
    public class FrameExporterFactory : IFactoryFormat
    {
        public IFrameExporter CreateExporter(ExportFormat format)
        {
            switch (format)
            {
                case ExportFormat.Json:
                    return new JsonFrameExporter();

                case ExportFormat.Csv:
                    return new CsvFrameExporter();

                case ExportFormat.Pcap:
                    return new PcapFrameExporter();

                default:
                    throw new NotSupportedException($"Unsupported export format: {format}");
            }
        }
    }
}
