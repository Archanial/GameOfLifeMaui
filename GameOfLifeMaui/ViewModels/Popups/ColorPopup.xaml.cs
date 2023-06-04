using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui.ViewModels.Popups;

public sealed partial class ColorPopup
{
    private const string ColorFieldString = "Color:";

    private Color _color = new(255, 255, 255);
    
    public ColorPopup(int? age = null, Color color = null)
    {
        InitializeComponent();
        
        if (age.HasValue)
        {
            AgeField.Text = age.ToString();
        }

        if (color != null)
        {
            //ColorPicker.SetPointerRingPosition(color.);
        }
    }

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(AgeField.Text, out var age) || age <= 0)
        {
            return;
        }
        await SettingsManager.AddOrUpdateColor(age, _color);
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.BuildColors();
        }
        
        Close();
    }
    
    private void CancelButtonClicked(object sender, EventArgs e) => Close();

    private void OnPickedColorChanged(object sender, Color e)
    {
        _color = e;
        ColorField.Text = $"{ColorFieldString} {_color.ToColorString()}";
    }

    private void AgeFieldOnCompleted(object sender, EventArgs e) => AgeField.Unfocus();
}