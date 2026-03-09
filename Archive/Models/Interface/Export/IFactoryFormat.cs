using Archive.Models.Enum;

namespace Archive.Models.Interface.Export
{
    public interface IFactoryFormat
    {
        IFrameExporter CreateExporter(ExportFormat format);
    }
}
