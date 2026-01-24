namespace Archive.Models.Dto
{
    public class existingFlight
    {
        public int flightNumber { get; set; }
        public int flightLenght { get; set; }


        public existingFlight(int flightNumber, int flightLenght)
        {
            this.flightNumber = flightNumber;
            this.flightLenght = flightLenght;

        }
    }
}
