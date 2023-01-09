using CommunityToolkit.Maui.Alerts;

namespace GameOfLifeMaui;

public partial class SettingsPage
{
    private const string CellSizeSampleText = "Current cell size";
    private const int MaxCellSizeInPx = 300;
    
    public SettingsPage()
    {
        InitializeComponent();
        
        CellSize.Text = $"{CellSizeSampleText} {SettingsManager.CurrentCellSize}";
    }

    private async void OnCellSizeClick(object sender, EventArgs eventArgs)
    {
        var result = await DisplayPromptAsync(
            "New cell size (in px)", 
            "Warning! Small cell sizes may have an impact on performance!\nChanging this will take a while!", 
            keyboard : Keyboard.Numeric );

        if (!int.TryParse(result, out var parsed))
        {
            await Toast.Make("Invalid value entered").Show();
            return;
        }

        if (parsed is <= SettingsManager.MinCellSize or >= 50)
        {
            await Toast.Make("Entered value too small or too big").Show();
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(async () => await SettingsManager.ChangeCellSize(parsed));
        CellSize.Text = $"{CellSizeSampleText} {parsed}";
    }
}