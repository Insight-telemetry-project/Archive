using Archive.Models.Dto;
using Archive.Models.Interface.Export;
using Archive.Models.Schema;
using System.IO.Compression;
using System.Text;

namespace Archive.Services.Export
{
    public class PcapFrameExporter : IFrameExporter
    {
        private BinaryWriter _writer;

        public Task StartAsync(
            ZipArchive archive,
            int flightId,
            FlightSuspiciousPointsDto specialPoints,
            Dictionary<string, List<string>> connections,
            Dictionary<string, int> fields)
        {
            ZipArchiveEntry entry = archive.CreateEntry($"flight_{flightId}.pcap");

            Stream stream = entry.Open();

            _writer = new BinaryWriter(stream, Encoding.UTF8, true);


            _writer.Write(0xa1b2c3d4);
            _writer.Write((ushort)2);
            _writer.Write((ushort)4);
            _writer.Write(0);
            _writer.Write(0);
            _writer.Write(65535);
            _writer.Write(1);

            return Task.CompletedTask;
        }

        public Task WriteFrameAsync(TelemetrySensorFields frame)
        {
            MemoryStream payloadStream = new MemoryStream();
            BinaryWriter payloadWriter = new BinaryWriter(payloadStream);

            payloadWriter.Write(frame.Timestep);
            payloadWriter.Write(frame.Fields.Count);

            foreach (KeyValuePair<string, double> pair in frame.Fields)
            {
                payloadWriter.Write(pair.Key.Length);
                payloadWriter.Write(Encoding.ASCII.GetBytes(pair.Key));
                payloadWriter.Write(pair.Value);
            }

            payloadWriter.Flush();

            byte[] telemetryPayload = payloadStream.ToArray();

            MemoryStream packetStream = new MemoryStream();
            BinaryWriter packetWriter = new BinaryWriter(packetStream);

            packetWriter.Write(new byte[6]);
            packetWriter.Write(new byte[6]);
            packetWriter.Write(new byte[] { 0x08, 0x00 });

            packetWriter.Write((byte)0x45);
            packetWriter.Write((byte)0);
            packetWriter.Write((ushort)(20 + 8 + telemetryPayload.Length));
            packetWriter.Write((ushort)0);
            packetWriter.Write((ushort)0);
            packetWriter.Write((byte)64);
            packetWriter.Write((byte)17);
            packetWriter.Write((ushort)0);
            packetWriter.Write((uint)0x0100007F);
            packetWriter.Write((uint)0x0100007F);

            packetWriter.Write((ushort)5000);
            packetWriter.Write((ushort)5001);
            packetWriter.Write((ushort)(8 + telemetryPayload.Length));
            packetWriter.Write((ushort)0);

            packetWriter.Write(telemetryPayload);

            packetWriter.Flush();

            byte[] packetData = packetStream.ToArray();

            uint seconds = (uint)(frame.Timestep / 1000);
            uint microseconds = (uint)((frame.Timestep % 1000) * 1000);

            _writer.Write(seconds);
            _writer.Write(microseconds);
            _writer.Write(packetData.Length);
            _writer.Write(packetData.Length);

            _writer.Write(packetData);

            return Task.CompletedTask;
        }

        public Task EndAsync()
        {
            _writer.Flush();
            _writer.Dispose();

            return Task.CompletedTask;
        }
    }
}
