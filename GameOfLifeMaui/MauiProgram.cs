using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace GameOfLifeMaui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        })
            .UseMauiCommunityToolkit()
            .UseSkiaSharp();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        
        return builder.Build();
    }
}