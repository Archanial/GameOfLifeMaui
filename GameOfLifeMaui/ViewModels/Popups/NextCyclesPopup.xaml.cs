using CommunityToolkit.Maui.Alerts;
using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui.ViewModels.Popups;

public sealed partial class NextCyclesPopup
{
    private const int MaxCycles = 1000;
    
    public NextCyclesPopup()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void CancelButtonClicked(object sender, EventArgs e) => Close();

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(SizeLabel.Text, out var parsed))
        {
            return;
        }

        if (parsed is <= 0 or >= MaxCycles)
        {
            await Toast.Make("Entered value too small or too big").Show();
            return;
        }
        
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.OnNextFramesChanged(parsed);
        }

        await SettingsManager.ChangeNextButtonFrames(parsed);
        Close();
    }
}