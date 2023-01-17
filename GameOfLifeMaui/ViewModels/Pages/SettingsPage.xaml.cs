using CommunityToolkit.Maui.Views;
using GameOfLifeMaui.ViewModels.Popups;

namespace GameOfLifeMaui.ViewModels.Pages;

public partial class SettingsPage
{
    private const string RulestringText = "Current rulestring:";
    
    private const string CellSizeSampleText = "Current cell size:";
    
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

        TappedAgeChange();
        OnSizeChanged(SettingsManager.CurrentCellSize);
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

    private async void OnCellSizeClick(object sender, EventArgs eventArgs) 
        => await Shell.Current.ShowPopupAsync(new CellSizePopup());

    private static async void AddColor(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new ColorPopup());
    
    private async void ChangeRulestring(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new RulestringPopup());

    private async void TappedCellAgeChange(object sender, TappedEventArgs e) 
        => await Shell.Current.ShowPopupAsync(new CellAgePopup());
}