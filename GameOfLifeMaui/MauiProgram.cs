using CommunityToolkit.Maui;
using GameOfLifeMaui.ViewModels;
using GameOfLifeMaui.ViewModels.Pages;
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
        builder.Services.AddSingleton<IDatabase, GoLDatabase>();
        
        return builder.Build();
    }
}