using Archive.Models.Enum;

namespace Archive.Models.Interface.Export
{
    public interface IExportService
    {
        Task<Stream> ExportFlightAsync(int masterIndex, ExportFormat format);

    }
}
