using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using GameOfLifeMaui.Database;
using GameOfLifeMaui.ViewModels;
using GameOfLifeMaui.ViewModels.Pages;
using GameOfLifeMaui.ViewModels.Popups;
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

        //Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        
        //Popups
        builder.Services.AddTransient<RulestringPopup>();
        builder.Services.AddTransient<ColorPopup>();
        builder.Services.AddTransient<CellAgePopup>();
        builder.Services.AddTransient<NextCyclesPopup>();
        builder.Services.AddTransient<CellSizePopup>();

        //Other
        builder.Services.AddSingleton<IDatabase, GoLDatabase>();
        builder.Services.AddSingleton(FileSaver.Default);
        builder.Services.AddSingleton(FolderPicker.Default);
        builder.Services.AddSingleton(FilePicker.Default);
        builder.Services.AddSingleton<Game>();
        
        var app = builder.Build();
        Initialize(app);
        return app;
    }
    
    private static void Initialize(MauiApp program)
    {
        //Init statics
        var gameInstance = program.Services.GetRequiredService<Game>();
        var mainPageInstance = program.Services.GetRequiredService<MainPage>();
        SettingsManager.Initialize(gameInstance,
            mainPageInstance,
            program.Services.GetService<IDatabase>());
        PanGestureHandler.Initialize(gameInstance, mainPageInstance);
    }
}