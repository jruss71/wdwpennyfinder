using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using System.Collections.Generic;
using WDWPennyFinder;

namespace WDWPennyFinder
{
    public class CustomMap : Microsoft.Maui.Controls.Maps.Map
    {
        public List<CustomPin> CustomPins { get; set; }
        public CustomMap() { }

        public MapPage mapPage
        {
            get; set;
        }
    }
}