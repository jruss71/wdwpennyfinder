using Microsoft.Maui.Controls.Maps;

namespace WDWPennyFinder
{
    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Machine { get; set; }
        public string LocationName { get; set; }
        public int MachineID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Location Position { get; set; }
        public Location PinPosition
        {
            get
            {
                return new Microsoft.Maui.Devices.Sensors.Location(Latitude, Longitude);
            }

        }
    }
}
