namespace BusBoard.ConsoleApp
{
    public class ResultData
    {
        public PostcodeCoords Result { get; set; }
    }

    public class PostcodeCoords
    {
        public string Postcode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
