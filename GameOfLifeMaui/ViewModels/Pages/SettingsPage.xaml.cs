using CommunityToolkit.Maui.Views;
using GameOfLifeMaui.Fractals;
using GameOfLifeMaui.Models.Enums;
using GameOfLifeMaui.ViewModels.Popups;

namespace GameOfLifeMaui.ViewModels.Pages;

public sealed partial class SettingsPage
{
    private const string RulestringText = "Current rulestring:";
    
    private const string CellSizeSampleText = "Current cell size:";
    
    private const string GenerationsPerClickText = "Generations per click:";
    
    private const string TappedCellAgeText = "Tapped cell age:";

    private static Button _addColorButton;
    
    public SettingsPage()
    {
        InitializeComponent();

        if (_addColorButton == null)
        {
            _addColorButton = new Button
            {
                ImageSource = GetSymbolImagePath(),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 30,
                WidthRequest = 30,
                BackgroundColor = new Color(0, 0, 0, 0),
                Margin = new Thickness
                {
                    Top = 5,
                    Bottom = 5
                }
            };
            _addColorButton.Clicked += AddColor;
            
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeChanged += CurrentOnRequestedThemeChanged;
            }
        }
        NextButtonCheckbox.IsChecked = 
            SettingsManager.GetMiscSetting(MiscSettingsBitflagEnum.ManualNextButtonShown);
        ScreenshotButtonCheckbox.IsChecked = 
            SettingsManager.GetMiscSetting(MiscSettingsBitflagEnum.ScreenshotButtonShown);

        UpdateGenerationsRow();
        TappedAgeChange();
        OnSizeChanged(SettingsManager.CurrentCellSize);
        OnNextFramesChanged(SettingsManager.NextButtonFrames);
        BuildColors();
        UpdateRulestring();
    }

    private static void CurrentOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e) =>
        _addColorButton.ImageSource = GetSymbolImagePath();

    private static string GetSymbolImagePath() =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? "Images/plus_icon.svg"
            : "Images/plus_icon_light.svg";

    public void BuildColors()
    {
        var holders = SettingsManager.GetColors().OrderBy(x => x.Key).ToList();
        ColorsSection.RowDefinitions = new RowDefinitionCollection();
        for (var i = 0; i < holders.Count + 1; i++)
        {
            ColorsSection.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }
        ColorsSection.Clear();
        for (var i = 0; i < holders.Count; i++)
        {
            ColorsSection.Add(new ColorHolder(holders[i].Value, holders[i].Key), 0, i);
        }

        ColorsSection.Add(_addColorButton, 0, holders.Count + 1);
    }

    public void UpdateRulestring() => Rulestring.Text = $"{RulestringText} {SettingsManager.GetRuleString}";

    public void TappedAgeChange() => TappedAge.Text = $"{TappedCellAgeText} {SettingsManager.TappedAge}";
    
    public void OnSizeChanged(int size) => CellSize.Text = $"{CellSizeSampleText} {size}";
    
    public void OnNextFramesChanged(int size) => NextButtonFrames.Text = $"{GenerationsPerClickText} {size}";

    private async void OnCellSizeClick(object sender, EventArgs eventArgs) 
        => await Shell.Current.ShowPopupAsync(new CellSizePopup());
    
    private async void OnNextButtonFramesClick(object sender, EventArgs eventArgs) 
        => await Shell.Current.ShowPopupAsync(new NextCyclesPopup());

    private static async void AddColor(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new ColorPopup());
    
    private async void ChangeRulestring(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new RulestringPopup());

    private async void TappedCellAgeChange(object sender, TappedEventArgs e) 
        => await Shell.Current.ShowPopupAsync(new CellAgePopup());
    
    private async void SaveToFile(object sender, TappedEventArgs e) 
        => await SettingsManager.SaveToFile();
    
    private async void ReadFromFile(object sender, TappedEventArgs e) 
        => await SettingsManager.ReadFromFile();

    private async void DrawCarpet(object sender, TappedEventArgs e)
    {
        var fractal = new SierpinskiCarpet(SettingsManager.Game);
        var result = await Shell.Current.ShowPopupAsync(new FractalPopup(fractal));
        if (result == null || !(bool)result) return;
        fractal.Act();
        await Navigation.PopAsync();
    }
    
    private async void DrawTriangle(object sender, TappedEventArgs e)
    {
        var fractal = new SierpinskiTriangle(SettingsManager.Game);
        var result = await Shell.Current.ShowPopupAsync(new FractalPopup(fractal));
        if (result == null || !(bool)result) return;
        fractal.Act();
        await Navigation.PopAsync();
    }

    private async void ToggleNextButton(object sender, TappedEventArgs e)
    {
        NextButtonCheckbox.IsChecked = !NextButtonCheckbox.IsChecked;
        await SettingsManager.SetMiscSetting(MiscSettingsBitflagEnum.ManualNextButtonShown,
            NextButtonCheckbox.IsChecked);
        UpdateGenerationsRow();
    }

    private void UpdateGenerationsRow()
    {
        GenerationsClickRow.Height = NextButtonCheckbox.IsChecked
            ? new GridLength(1,
                GridUnitType.Star)
            : new GridLength(0);
        NextButtonFrames.IsVisible = NextButtonCheckbox.IsChecked;
    }
    
    private async void ToggleScreenshotButton(object sender, TappedEventArgs e)
    {
        ScreenshotButtonCheckbox.IsChecked = !ScreenshotButtonCheckbox.IsChecked;
        await SettingsManager.SetMiscSetting(MiscSettingsBitflagEnum.ScreenshotButtonShown,
            ScreenshotButtonCheckbox.IsChecked);
    }
}