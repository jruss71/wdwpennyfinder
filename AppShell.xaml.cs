namespace WDWPennyFinder;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));

        Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));

    }

    void OnStackLayoutTapped(object sender, EventArgs e)
    {
        // Scroll to the top of the CollectionView
        //ItemsListView.ScrollTo(0, 0, ScrollToPosition.Start, true);
    }
}

