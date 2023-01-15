using CommunityToolkit.Maui.Alerts;

namespace GameOfLifeMaui.Popups;

public partial class CellAgePopup
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

        SettingsManager.TappedAge = parsed;
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.TappedAgeChange();
        }
        
        Close();
    }
}