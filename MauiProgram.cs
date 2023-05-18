using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace WDWPennyFinder;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder

                .UseMauiMaps()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.ConfigureMauiHandlers(handlers =>
        {
#if IOS 
            handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
        });


#if DEBUG
        builder.Logging.AddDebug();
#endif


        return builder.Build();
    }
}

