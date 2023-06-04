namespace GameOfLifeMaui.ViewModels;

public sealed partial class App
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}