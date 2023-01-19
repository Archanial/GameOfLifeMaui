using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Maui.Views;
using GameOfLifeMaui.ViewModels.Popups;

namespace GameOfLifeMaui.ViewModels.Pages;

public sealed partial class SettingsPage
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
        
        //USELESS
        ToggleAccelerometer();
        ToggleGyroscope();
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
    
    private void ToggleGyroscope()
    {
        GyroscopeField.Text = "Gyroscope not supported!";
        if (!Gyroscope.Default.IsSupported || Gyroscope.Default.IsMonitoring) return;
        Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
        Gyroscope.Default.Start(SensorSpeed.UI);
    }

    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e) 
        => GyroscopeField.Text = $"Gyroscope: X:{e.Reading.AngularVelocity.X:n2} Y:{e.Reading.AngularVelocity.Y:n2} Z:{e.Reading.AngularVelocity.Z:n2}";

    private void ToggleAccelerometer()
    {
        AccelerometerField.Text = "Accelerometer not supported!";
        if (!Accelerometer.Default.IsSupported || Accelerometer.Default.IsMonitoring) return;
        Accelerometer.Default.ReadingChanged += AccelerometerReadingChanged;
        Accelerometer.Default.Start(SensorSpeed.UI);
    }

    private void AccelerometerReadingChanged(object sender, AccelerometerChangedEventArgs e) 
        => AccelerometerField.Text = $"Accel: X:{e.Reading.Acceleration.X:n2} Y:{e.Reading.Acceleration.Y:n2} Z:{e.Reading.Acceleration.Z:n2}";

    private async void OnCellSizeClick(object sender, EventArgs eventArgs) 
        => await Shell.Current.ShowPopupAsync(new CellSizePopup());

    private static async void AddColor(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new ColorPopup());
    
    private async void ChangeRulestring(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new RulestringPopup());

    private async void TappedCellAgeChange(object sender, TappedEventArgs e) 
        => await Shell.Current.ShowPopupAsync(new CellAgePopup());
    
    public async Task GetTime()
    {
        var uri = new Uri("https://worldtimeapi.org/api/timezone/Europe/Warsaw");
        var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TimeItem>(content, new JsonSerializerOptions());
                TimeField.Text = result.Datetime.ToString(CultureInfo.InvariantCulture);
            }
        }
        catch (Exception)
        {
            TimeField.Text = "Error";
        }
    }
}

file sealed class TimeItem
{
    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; }
}