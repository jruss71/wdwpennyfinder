namespace WDWPennyFinder;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps.Platform;
using Microsoft.Maui.Platform;
using UIKit;
using WDWPennyFinder;
using WDWPennyFinder.Platforms.iOS;
using WDWPennyFinder.ViewModels;


public class CustomMapHandler : MapHandler
{
    private static UIView? lastTouchedView;

    public string ItemName { get; set; }

    public string LocationName { get; set; }

    public string MachineName { get; set; }

    public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
        new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins,
        };

    public CustomMapHandler() : base(CustomMapper, CommandMapper)
    {
    }

    public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
        mapper ?? CustomMapper, commandMapper ?? CommandMapper)
    {
    }

    public List<IMKAnnotation> Markers { get; } = new();

    protected override void ConnectHandler(MauiMKMapView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.GetViewForAnnotation += GetViewForAnnotations;
    }

    private static void OnCalloutClicked(IMKAnnotation annotation)
    {
        var pin = GetPinForAnnotation(annotation);
        if (lastTouchedView is MKAnnotationView)
            return;
        pin?.SendInfoWindowClick();
    }


    private static async void OnDirectionsLinkTapped(CustomAnnotation selectedAnnotation)
    {

        //if (mapView?.UserLocation?.Location == null)
        //{
        //    // User location is not available
        //    return;
        //}

        var destinationCoordinate = new Location(selectedAnnotation.Coordinate.Latitude, selectedAnnotation.Coordinate.Longitude);
        var options = new MapLaunchOptions { NavigationMode = NavigationMode.Walking };

        await Map.Default.OpenAsync(destinationCoordinate, options);
    }


    private static MKAnnotationView GetViewForAnnotations(MKMapView mapView, IMKAnnotation annotation)
    {
        MKAnnotationView annotationView;
        if (annotation is CustomAnnotation customAnnotation)
        {
            annotationView = mapView.DequeueReusableAnnotation(customAnnotation.Identifier.ToString()) ??
                             new MKAnnotationView(annotation, customAnnotation.Identifier.ToString());

            annotationView.Image = UIImage.FromFile("pin.png");
            customAnnotation.Title = null;
            customAnnotation.Subtitle = null;
            //annotationView.DetailCalloutAccessoryView =
            CreateCustomCalloutView(annotationView, customAnnotation);
            annotationView.CanShowCallout = true;

        }

        else if (annotation is MKPointAnnotation)
        {
            annotationView = mapView.DequeueReusableAnnotation("defaultPin") ??
                             new MKMarkerAnnotationView(annotation, "defaultPin");
            annotationView.CanShowCallout = false;
        }
        else
        {
            annotationView = new MKUserLocationView(annotation, null);
        }

        var result = annotationView ?? new MKAnnotationView(annotation, null);
        AttachGestureToPin(result, annotation);
        return result;
    }

    private static void CreateCustomCalloutView(MKAnnotationView mapPin, CustomAnnotation annotation)
    {
        mapPin.CanShowCallout = true;
        mapPin.DetailCalloutAccessoryView = new UIView();
        var locationLabel = new UILabel
        {
            Text = annotation?.LocationName,
            Font = UIFont.BoldSystemFontOfSize(14)
        };

        var machineLabel = new UILabel
        {
            Text = annotation?.MachineName,
            Font = UIFont.SystemFontOfSize(12)
        };

        var itemLabel = new UILabel
        {
            Text = annotation?.ItemName,
            Font = UIFont.SystemFontOfSize(12),
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap
        };

        var hyperlinkLabel = new UILabel(new CGRect(0, 25, 200, 20))
        {
            Text = "Get Directions",
            TextColor = UIColor.Blue,
            Font = UIFont.SystemFontOfSize(12),
            UserInteractionEnabled = true
        };
        var tapGesture = new UITapGestureRecognizer();
        tapGesture.AddTarget(() => OnDirectionsLinkTapped(annotation));
        hyperlinkLabel.AddGestureRecognizer(tapGesture);

        var stackView = new UIStackView(new[] { locationLabel, machineLabel, itemLabel, hyperlinkLabel });
        stackView.Axis = UILayoutConstraintAxis.Vertical;
        stackView.Distribution = UIStackViewDistribution.EqualSpacing;
        stackView.TranslatesAutoresizingMaskIntoConstraints = false;

        mapPin.DetailCalloutAccessoryView.AddSubview(stackView);
        stackView.WidthAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.WidthAnchor).Active = true;
        stackView.HeightAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.HeightAnchor).Active = true;
        stackView.CenterXAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.CenterXAnchor).Active = true;
        stackView.CenterYAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.CenterYAnchor).Active = true;
        // Set the right accessory view to the information button
        var infoButton = UIButton.FromType(UIButtonType.DetailDisclosure);
        mapPin.RightCalloutAccessoryView = infoButton;
        var color = Microsoft.Maui.Graphics.Color.FromArgb("88eb6");
        string hexColor = "#886EB6";
        UIColor uiColor = UIColor.FromRGB(
            Convert.ToInt32(hexColor.Substring(1, 2), 16),
            Convert.ToInt32(hexColor.Substring(3, 2), 16),
            Convert.ToInt32(hexColor.Substring(5, 2), 16)
        );
        infoButton.TintColor = uiColor;
        infoButton.TouchUpInside += async (sender, e) =>
        {
            var viewModel = new ItemsViewModel();
            var customAnnotation = annotation as CustomAnnotation;
            if (customAnnotation != null)
            {
                await viewModel.CheckBoxItemsForMachine(customAnnotation.MachineID);
                Console.WriteLine(viewModel.CheckBoxItems);
                var checkboxContentPage = new CheckBoxContentPage(viewModel.CheckBoxItems);

                // Get the current top-level page to display the popup
                var currentTopPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault()
                    ?? Application.Current.MainPage;

                currentTopPage.ShowPopup(checkboxContentPage);
            }

        };

        // Set the left callout accessory to an image
        var imageName = "resort_48x48.png";
        var customAnnotation = annotation as CustomAnnotation;
        if (customAnnotation?.LocationName?.Equals("Animal Kingdom", StringComparison.OrdinalIgnoreCase) == true)
        {
            imageName = "ak_48x48.png";
        }
        else if (customAnnotation?.LocationName?.Equals("Magic Kingdom", StringComparison.OrdinalIgnoreCase) == true)
        {
            imageName = "mk_48x48.png";
        }
        else if (customAnnotation?.LocationName?.Equals("Epcot", StringComparison.OrdinalIgnoreCase) == true)
        {
            imageName = "epcot_48x48.png";
        }
        else if (customAnnotation?.LocationName?.Equals("Hollywood Studios", StringComparison.OrdinalIgnoreCase) == true)
        {
            imageName = "hs_48x48.png";
        }

        var image = UIImage.FromFile(imageName);
        var imageView = new UIImageView(image);
        mapPin.LeftCalloutAccessoryView = imageView;
    }


    static void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
    {
        var recognizers = mapPin.GestureRecognizers;
        if (recognizers != null)
        {
            foreach (var r in recognizers)
            {
                mapPin.RemoveGestureRecognizer(r);
            }
        }

        var recognizer = new UITapGestureRecognizer(g =>
        {
            // Show callout with text when image is clicked
            //mapPin.CanShowCallout = true;
            //mapPin.DetailCalloutAccessoryView = new UIView();

            //var locationLabel = new UILabel
            //{
            //    Text = (annotation as CustomAnnotation)?.LocationName,
            //    Font = UIFont.BoldSystemFontOfSize(14)
            //};

            //var machineLabel = new UILabel
            //{
            //    Text = (annotation as CustomAnnotation)?.MachineName,
            //    Font = UIFont.SystemFontOfSize(12)
            //};

            //var itemLabel = new UILabel
            //{
            //    Text = (annotation as CustomAnnotation)?.ItemName,
            //    Font = UIFont.SystemFontOfSize(12),
            //    Lines = 0,
            //    LineBreakMode = UILineBreakMode.WordWrap
            //};

            //var hyperlinkLabel = new UILabel(new CGRect(0, 25, 200, 20))
            //{
            //    Text = "Get Directions",
            //    TextColor = UIColor.Blue,
            //    Font = UIFont.SystemFontOfSize(12),
            //    UserInteractionEnabled = true
            //};
            //var tapGesture = new UITapGestureRecognizer(OnDirectionsLinkTapped);
            //hyperlinkLabel.AddGestureRecognizer(tapGesture);

            //var stackView = new UIStackView(new[] { locationLabel, machineLabel, itemLabel, hyperlinkLabel });
            //stackView.Axis = UILayoutConstraintAxis.Vertical;
            //stackView.Distribution = UIStackViewDistribution.EqualSpacing;
            //stackView.TranslatesAutoresizingMaskIntoConstraints = false;

            //mapPin.DetailCalloutAccessoryView.AddSubview(stackView);
            //stackView.WidthAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.WidthAnchor).Active = true;
            //stackView.HeightAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.HeightAnchor).Active = true;
            //stackView.CenterXAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.CenterXAnchor).Active = true;
            //stackView.CenterYAnchor.ConstraintEqualTo(mapPin.DetailCalloutAccessoryView.CenterYAnchor).Active = true;
            //// Set the right accessory view to the information button
            //var infoButton = UIButton.FromType(UIButtonType.DetailDisclosure);
            //mapPin.RightCalloutAccessoryView = infoButton;
            //var color = Microsoft.Maui.Graphics.Color.FromArgb("88eb6");
            //string hexColor = "#886EB6";
            //UIColor uiColor = UIColor.FromRGB(
            //    Convert.ToInt32(hexColor.Substring(1, 2), 16),
            //    Convert.ToInt32(hexColor.Substring(3, 2), 16),
            //    Convert.ToInt32(hexColor.Substring(5, 2), 16)
            //);
            //infoButton.TintColor = uiColor;
            //infoButton.TouchUpInside += async (sender, e) =>
            //{
            //    var viewModel = new ItemsViewModel();
            //    var customAnnotation = annotation as CustomAnnotation;
            //    if (customAnnotation != null)
            //    {
            //        await viewModel.CheckBoxItemsForMachine(customAnnotation.MachineID);
            //        Console.WriteLine(viewModel.CheckBoxItems);
            //        var checkboxContentPage = new CheckBoxContentPage(viewModel.CheckBoxItems);

            //        // Get the current top-level page to display the popup
            //        var currentTopPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault()
            //            ?? Application.Current.MainPage;

            //        currentTopPage.ShowPopup(checkboxContentPage);
            //    }

            //};

            //// Set the left callout accessory to an image
            //var imageName = "resort_48x48.png";
            //var customAnnotation = annotation as CustomAnnotation;
            //if (customAnnotation?.LocationName?.Equals("Animal Kingdom", StringComparison.OrdinalIgnoreCase) == true)
            //{
            //    imageName = "ak_48x48.png";
            //}
            //else if (customAnnotation?.LocationName?.Equals("Magic Kingdom", StringComparison.OrdinalIgnoreCase) == true)
            //{
            //    imageName = "mk_48x48.png";
            //}
            //else if (customAnnotation?.LocationName?.Equals("Epcot", StringComparison.OrdinalIgnoreCase) == true)
            //{
            //    imageName = "epcot_48x48.png";
            //}
            //else if (customAnnotation?.LocationName?.Equals("Hollywood Studios", StringComparison.OrdinalIgnoreCase) == true)
            //{
            //    imageName = "hs_48x48.png";
            //}

            //var image = UIImage.FromFile(imageName);
            //var imageView = new UIImageView(image);
            //mapPin.LeftCalloutAccessoryView = imageView;
        })
        {
            ShouldReceiveTouch = (gestureRecognizer, touch) =>
            {
                lastTouchedView = touch.View;
                return true;
            }
        };

        mapPin.AddGestureRecognizer(recognizer);
    }

    static IMapPin? GetPinForAnnotation(IMKAnnotation? annotation)
    {
        if (annotation is CustomAnnotation customAnnotation)
        {
            return customAnnotation.Pin;
        }

        return null;
    }

    private static new void MapPins(IMapHandler handler, IMap map)
    {
        if (handler is CustomMapHandler mapHandler)
        {
            foreach (var marker in mapHandler.Markers)
            {
                mapHandler.PlatformView.RemoveAnnotation(marker);
            }

            mapHandler.Markers.Clear();
            mapHandler.AddPins(map.Pins);
        }
    }

    private void AddPins(IEnumerable<IMapPin> mapPins)
    {
        if (MauiContext is null)
        {
            return;
        }

        foreach (var pin in mapPins)
        {
            var pinHandler = pin.ToHandler(MauiContext);
            if (pinHandler is IMapPinHandler mapPinHandler)
            {
                var markerOption = mapPinHandler.PlatformView;
                if (pin is CustomPin cp)
                {
                    markerOption = new CustomAnnotation()
                    {
                        Identifier = cp.Id,
                        Title = cp.Machine,
                        Subtitle = cp.Machine,
                        LocationName = cp.LocationName,
                        ItemName = cp.Name,
                        MachineName = cp.Machine,
                        MachineID = cp.MachineID,
                        Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude),
                        Pin = cp
                    };

                    AddMarker(PlatformView, pin, Markers, markerOption);
                }
                else
                {
                    AddMarker(PlatformView, pin, Markers, markerOption);
                }
            }
        }
    }

    private static void AddMarker(MauiMKMapView map, IMapPin pin, List<IMKAnnotation> markers, IMKAnnotation annotation)
    {
        map.AddAnnotation(annotation);
        pin.MarkerId = annotation;
        markers.Add(annotation);
    }
}