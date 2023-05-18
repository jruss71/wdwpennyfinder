using System;
using System.Collections.Generic;
using WDWPennyFinder.Models;
using WDWPennyFinder.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using WDWPennyFinder.Converters;

namespace WDWPennyFinder
{
    public partial class MapPage : ContentPage
    {
        private ItemDetail itemDetail;
        private ObservableCollection<ItemDetail> items;
        private ItemsViewModel _viewModel;

        private readonly double defaultZoomLevel = 16;

        public MapPage()
        {
            InitializeComponent();
            _viewModel = new ItemsViewModel();

            slider = (Slider)FindByName("slider");
            _viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
            Resources.Add("LocationParkConverter", new LocationParkConverter());
            slider.Value = defaultZoomLevel;
            //customMap.MapClicked += CustomMap_MapClicked;
        }

        private async Task LoadItems()
        {
            await _viewModel.ExecuteLoadItemsCommand();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            ItemDetail previousItem;

            if (BindingContext is ItemDetail vm)
            {
                itemDetail = vm;

                var pin = new CustomPin
                {
                    Position = itemDetail.Machine.pinPosition, //pinPosition,

                    Name = await itemDetail.GetOtherPenniesAtLocation(),
                    Label = string.Empty,
                    Machine = itemDetail.Machine.name,
                    Location = new Microsoft.Maui.Devices.Sensors.Location(itemDetail.Machine.latitude, itemDetail.Machine.longitude),
                    MachineID = itemDetail.Machine.Id,
                    LocationName = itemDetail.Location.name,
                    Latitude = itemDetail.Machine.latitude,
                    Longitude = itemDetail.Machine.longitude
                };
                customMap.CustomPins = new List<CustomPin> { pin };

                customMap.Pins.Add(pin);
                customMap.mapPage = this;
                var mapSpan = MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(pin.Latitude, pin.Longitude), Distance.FromMiles(0.5));
                customMap.MoveToRegion(mapSpan);

                // Set the slider value to match the default zoom level of the map
                var latlongDegrees = 360 / (Math.Pow(2, defaultZoomLevel));
                slider.Value = defaultZoomLevel;
                if (customMap.VisibleRegion != null)
                {
                    customMap.MoveToRegion(new MapSpan(customMap.VisibleRegion.Center, latlongDegrees, latlongDegrees));
                }
            }
            else if (BindingContext is ItemsViewModel vmList)
            {
                var mapSpan = MapSpan.FromCenterAndRadius(
                 new Microsoft.Maui.Devices.Sensors.Location(28.385233, -81.56), Distance.FromMiles(3.0));
                customMap.MoveToRegion(mapSpan);

                await LoadItems();
                customMap.CustomPins = new List<CustomPin>();
                // This is the list view so lets add pins for the full list
                var itemDetails = new ObservableCollection<ItemDetail>(
                    vmList.Items
                    .OrderBy(item => item.Location.name)
                    .ThenBy(item => item.Item.Name));

                // we initialize the PrevLocation for our first item so it won't automatically be seen as a new "group"
                if (itemDetails.Count > 0)
                {
                    previousItem = itemDetails.First();
                    String pennyName = previousItem.Item.Name;
                    foreach (ItemDetail itemDetail in itemDetails)
                    {
                        if (itemDetail.Machine.name != previousItem.Machine.name)
                        {
                            var pin = new CustomPin
                            {
                                Position = previousItem.Machine.pinPosition,
                                Name = pennyName,
                                Machine = previousItem.Machine.name,
                                Location = new Microsoft.Maui.Devices.Sensors.Location(itemDetail.Machine.latitude, itemDetail.Machine.longitude),

                                LocationName = previousItem.Location.name,
                                MachineID = previousItem.Machine.Id,
                                Latitude = previousItem.Machine.latitude,
                                Longitude = previousItem.Machine.longitude,
                                Label = string.Empty,
                                Type = PinType.Place

                            };
                            pennyName = itemDetail.Item.Name;

                            customMap.CustomPins.Add(pin);
                            customMap.Pins.Add(pin);
                        }
                        else
                        {
                            if (previousItem.Item.Name != itemDetail.Item.Name)
                            {
                                pennyName += "\n" + itemDetail.Item.Name;
                                Console.WriteLine(pennyName);
                            }
                        }
                        previousItem = itemDetail;
                    }
                }
                customMap.mapPage = this;
                var latlongDegrees = 360 / (Math.Pow(2, defaultZoomLevel));
                slider.Value = defaultZoomLevel;


            }
        }



        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (customMap != null)
            {
                double zoomLevel = e.NewValue;
                double latlongDegrees = 360 / (Math.Pow(2, zoomLevel));
                if (customMap.VisibleRegion != null)
                {
                    customMap.MoveToRegion(new MapSpan(customMap.VisibleRegion.Center, latlongDegrees, latlongDegrees));
                }
            }
        }


        void OnButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            switch (button.Text)
            {
                case "Street":
                    customMap.MapType = MapType.Street;
                    break;
                case "Satellite":
                    customMap.MapType = MapType.Satellite;
                    break;
                case "Hybrid":
                    customMap.MapType = MapType.Hybrid;
                    break;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            customMap.Pins.Clear();
        }
    }

}
