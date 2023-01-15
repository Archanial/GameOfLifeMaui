using CommunityToolkit.Maui.Alerts;

namespace GameOfLifeMaui.Popups;

public partial class CellSizePopup
{
    public CellSizePopup()
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

        if (parsed is <= SettingsManager.MinCellSize or >= SettingsManager.MaxCellSize)
        {
            await Toast.Make("Entered value too small or too big").Show();
            return;
        }

        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.OnSizeChanged(parsed);
        }

        await MainThread.InvokeOnMainThreadAsync(async () 
            => await SettingsManager.ChangeCellSize(parsed));
        Close();
    }
}