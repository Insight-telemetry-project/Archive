namespace Archive.Models.Dto
{
    public class CreateInvestigationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MasterIndex { get; set; }
        public string Param { get; set; } = string.Empty;
        public long Time { get; set; }
        public double Value { get; set; }
    }
}
