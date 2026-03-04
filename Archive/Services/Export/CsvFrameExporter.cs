using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Interface.Export;
using Archive.Models.Schema;
using System.IO.Compression;
using System.Text;

namespace Archive.Services.Export
{
    public class CsvFrameExporter : IFrameExporter
    {
        private StreamWriter _writer;

        private List<string> _parameters;
        private bool _headerWritten = false;
        public Task StartAsync(
    ZipArchive archive,
    int flightId,
    FlightSuspiciousPointsDto specialPoints,
    Dictionary<string, List<string>> connections,
    Dictionary<string, int> fields)
        {

            ZipArchiveEntry anomaliesEntry = archive.CreateEntry("anomalies.csv");

            using (Stream anomaliesStream = anomaliesEntry.Open())
            using (StreamWriter anomaliesWriter = new StreamWriter(anomaliesStream, Encoding.UTF8))
            {
                anomaliesWriter.WriteLine("parameter,timestep");

                foreach (KeyValuePair<string, List<long>> pair in specialPoints.Anomalies)
                {
                    foreach (long ts in pair.Value)
                    {
                        anomaliesWriter.Write(pair.Key);
                        anomaliesWriter.Write(",");
                        anomaliesWriter.WriteLine(ts);
                    }
                }
            }



            ZipArchiveEntry connectionsEntry = archive.CreateEntry("connections.csv");

            using (Stream connectionsStream = connectionsEntry.Open())
            using (StreamWriter connectionsWriter = new StreamWriter(connectionsStream, Encoding.UTF8))
            {
                connectionsWriter.WriteLine("parameter,connected_to");

                foreach (KeyValuePair<string, List<string>> pair in connections)
                {
                    foreach (string target in pair.Value)
                    {
                        connectionsWriter.Write(pair.Key);
                        connectionsWriter.Write(",");
                        connectionsWriter.WriteLine(target);
                    }
                }
            }



            ZipArchiveEntry similarityEntry = archive.CreateEntry("historical_similarity.csv");

            using (Stream similarityStream = similarityEntry.Open())
            using (StreamWriter similarityWriter = new StreamWriter(similarityStream, Encoding.UTF8))
            {
                similarityWriter.WriteLine("parameter,startIndex,endIndex,label,score,comparedFlight");

                foreach (KeyValuePair<string, List<HistoricalSimilarityPoint>> pair in specialPoints.HistoricalSimilarity)
                {
                    foreach (HistoricalSimilarityPoint point in pair.Value)
                    {
                        similarityWriter.Write(pair.Key);
                        similarityWriter.Write(",");
                        similarityWriter.Write(point.StartIndex);
                        similarityWriter.Write(",");
                        similarityWriter.Write(point.EndIndex);
                        similarityWriter.Write(",");
                        similarityWriter.Write(point.Label);
                        similarityWriter.Write(",");
                        similarityWriter.Write(point.FinalScore);
                        similarityWriter.Write(",");
                        similarityWriter.WriteLine(point.ComparedFlightIndex);
                    }
                }
            }



            ZipArchiveEntry framesEntry = archive.CreateEntry("frames.csv");

            Stream outputStream = framesEntry.Open();

            _writer = new StreamWriter(outputStream, Encoding.UTF8, 8192, true);

            

            return Task.CompletedTask;
        }
        public Task WriteFrameAsync(TelemetrySensorFields frame)
        {
            if (!_headerWritten)
            {
                _parameters = new List<string>();

                foreach (string key in frame.Fields.Keys)
                {
                    _parameters.Add(key);
                }

                _writer.Write("timestep");

                foreach (string parameter in _parameters)
                {
                    _writer.Write(",");
                    _writer.Write(parameter);
                }

                _writer.WriteLine();

                _headerWritten = true;
            }

            _writer.Write(frame.Timestep);

            foreach (string parameter in _parameters)
            {
                _writer.Write(",");

                if (frame.Fields.ContainsKey(parameter))
                {
                    _writer.Write(frame.Fields[parameter]);
                }
            }

            _writer.WriteLine();

            return Task.CompletedTask;
        }

        public Task EndAsync()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}
