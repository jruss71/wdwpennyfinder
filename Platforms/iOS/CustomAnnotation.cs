using CoreLocation;
using MapKit;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Maps;
using UIKit;

namespace WDWPennyFinder.Platforms.iOS
{
    public class CustomAnnotation : MKPointAnnotation
    {
        public Guid Identifier { get; init; }
        public UIImage? Image { get; init; }
        public IMapPin Pin { get; init; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Location Location { get; set; }
        public string Machine { get; set; }
        public int MachineID { get; set; }
        public string LocationName { get; internal set; }
        public string ItemName { get; internal set; }
        public string MachineName { get; internal set; }
    }
}
