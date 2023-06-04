using CommunityToolkit.Maui.Alerts;
using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui.ViewModels.Popups;

public sealed partial class CellAgePopup
{
    public CellAgePopup()
    {
        InitializeComponent();
    }

    private void CancelButtonClicked(object sender, EventArgs e) => Close();

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(SizeLabel.Text, out var parsed))
        {
            return;
        }

        if (parsed <= 0)
        {
            await Toast.Make("Entered value should be higher than 0").Show();
            return;
        }

        await SettingsManager.ChangeTappedCellAge(parsed);
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.TappedAgeChange();
        }
        
        Close();
    }
}