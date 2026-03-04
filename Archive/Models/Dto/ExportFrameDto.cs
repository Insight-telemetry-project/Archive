namespace Archive.Models.Dto
{
    public class ExportFrameDto
    {
        public int Timestep { get; set; }

        public Dictionary<string, double> Values { get; set; }

        public bool IsAnomaly { get; set; }
    }
}
