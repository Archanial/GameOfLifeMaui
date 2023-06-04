using CommunityToolkit.Maui.Alerts;
using GameOfLifeMaui.Fractals;
using GameOfLifeMaui.ViewModels.Pages;
using static System.Int32;

namespace GameOfLifeMaui.ViewModels.Popups;

public sealed partial class FractalPopup
{
    private readonly Fractal _fractal;
    
    public FractalPopup(Fractal fractal)
    {
        _fractal = fractal;
        InitializeComponent();
    }

    private void CancelButtonClicked(object sender, EventArgs e) => Close(false);

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        if (TryParse(Height.Text, out var height))
        {
            if(height <= 0)
            {
                await Toast.Make("Entered value should be higher than 0").Show();
                return;
            }
        }

        if (TryParse(Width.Text, out var width))
        {
            if(width <= 0)
            {
                await Toast.Make("Entered value should be higher than 0").Show();
                return;
            }
        }

        if (TryParse(Iterations.Text, out var iter))
        {
            if (iter <= 0)
            {
                await Toast.Make("Entered value should be higher than 0").Show();
                return;
            }
        }
        
        _fractal.SetParameters(height, width, iter);
        Close(true);
    }
}