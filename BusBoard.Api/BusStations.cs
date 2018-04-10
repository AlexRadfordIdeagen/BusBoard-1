using System.Collections.Generic;

namespace BusBoard.ConsoleApp
{
    class ParentBusStop
    {
        public List<StopPoint> StopPoints { get; set; }
    }

    class StopPoint
    {
        public string CommonName { get; set; }
        public int Distance { get; set; }
        public List<LineGroup> LineGroup { get; set; }
    }
    class LineGroup
    {
        public string NaptanIdReference { get; set; }
    }
}
