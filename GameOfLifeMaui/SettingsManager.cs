using GameOfLifeMaui.Database;
using GameOfLifeMaui.Entities;
using GameOfLifeMaui.Models.Enums;
using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui;

public static class SettingsManager
{
    public static Game Game { get; private set; }
    
    public static int CurrentCellSize { get; private set; } = 30;

    public const int MinCellSize = 12;
    
    public const int MaxCellSize = 50;

    public static int TappedAge { get; private set; }
    
    public static int[] BArg { get; private set; }
    
    public static int[] SArg { get; private set; }
    
    public static int NextButtonFrames { get; private set; } = 1;

    private static IDatabase _database;
    
    private static SortedDictionary<int, Color> _colors = new();

    private static List<int> _keys = new();

    private static Dictionary<int, Color> _cachedColors;
    
    private static int _miscSettings = (int)MiscSettingsBitflagEnum.None;

    private static MainPage _mainPage;

    static SettingsManager()
    {
        TappedAge = 10;
        BArg = new[] { 3 };
        SArg = new[] { 2, 3 };
        BuildColorsCache();
    }

    public static async void Initialize(Game game, MainPage mainPage, IDatabase database)
    {
        Game = game;
        _mainPage = mainPage;
        _database = database;
        await LoadSettings();
    }

    public static bool GetMiscSetting(MiscSettingsBitflagEnum bitflag) => _miscSettings.HasFlag((int)bitflag);
    
    public static async Task SetMiscSetting(MiscSettingsBitflagEnum bitflag, bool isEnabled)
    {
        if (_miscSettings.HasFlag((int)bitflag) == isEnabled)
        {
            return;
        }
        if (isEnabled)
        {
            _miscSettings |= (int)bitflag;
        }
        else
        {
            _miscSettings &= ~(int)bitflag;
        }

        await HandleMiscSettings(bitflag, isEnabled);
    }
    
    public static async Task ChangeCellSize(int newSize, bool save = true)
    {
        CurrentCellSize = newSize;
        await Game.ChangeCellSize(newSize);
        if(save)
        {
            await SaveSetting(Constants.FieldNameCellSize, newSize.ToString());
        }
    }
    
    public static async Task ChangeNextButtonFrames(int value)
    {
        NextButtonFrames = value;
        await SaveSetting(Constants.FieldNextButtonFrames, value.ToString());
    }
    
    public static async Task ChangeTappedCellAge(int age)
    {
        TappedAge = age;
        await SaveSetting(Constants.FieldNameTappedAge, age.ToString());
    }

    public static IEnumerable<KeyValuePair<int, Color>> GetColors() => _colors.ToList();

    public static async Task AddOrUpdateColor(int age, Color color)
    {
        if (_colors.ContainsKey(age))
        {
            if (Equals(_colors[age], color))
            {
                return;
            }
            _colors[age] = color;
        }
        else
        {
            _colors.Add(age, color);
        }

        BuildColorsCache();
        Game.UpdateColors();
        await SaveColorSetting(age, color);
    }
    
    public static async Task TryRemoveColor(int age)
    {
        _colors.Remove(age);
        BuildColorsCache();
        Game.UpdateColors();
        await SaveColorSetting(age);
    }
    
    public static async Task SaveToFile() => await _mainPage.SaveLayoutToFile();

    public static async Task ReadFromFile() => await _mainPage.ReadLayoutFromFile();

    public static string GetRuleString => $"B{string.Join("", BArg)}/S{string.Join("", SArg)}";

    public static async Task SetRulestring(int[] b, int[] s, bool save = true)
    {
        if (b?.Any() ?? false)
        {
            BArg = b;
            if (save)
            {
                await SaveSetting(Constants.FieldNameBArg, b.ArrayToString());
            }
        }

        if (s?.Any() ?? false)
        {
            SArg = s;
            if(save)
            {
                await SaveSetting(Constants.FieldNameSArg, b.ArrayToString());
            }
        }
    }

    public static Color GetColor(int age)
    {
        if (!_keys.Any())
        {
            return age == 0 ? new Color(255, 255, 255) : new Color(0, 0, 0);
        }
        _cachedColors.TryGetValue(age, out var color);
        return color ?? _cachedColors[_keys.Last()];
    }

    private static async Task LoadSettings()
    {
        var settings = await _database.GetSettingsAsync();
        if (!settings?.Any() ?? true)
        {
            return;
        }

        foreach (var field in settings)
        {
            switch (field.FieldName)
            {
                case Constants.FieldNameBArg:
                    SetRulestring(field.Value, true, false);
                    break;
                case Constants.FieldNameSArg:
                    SetRulestring(field.Value, false, false);
                    break;
                case Constants.FieldNameTappedAge:
                    TappedAge = int.Parse(field.Value);
                    break;
                case Constants.FieldNameCellSize:
                    await MainThread.InvokeOnMainThreadAsync(async () 
                        => await ChangeCellSize(int.Parse(field.Value), false));
                    break;
                case Constants.FieldMiscSettings:
                    if (!int.TryParse(field.Value, out var miscSettings)) break;
                    _miscSettings = miscSettings;
                    await HandleMiscSettingsLoaded();
                    break;
                case Constants.FieldNextButtonFrames:
                    if (!int.TryParse(field.Value, out var nextButtonFrames)) break;
                    NextButtonFrames = nextButtonFrames;
                    break;
            }
        }
        
        var colors = await _database.GetColorsAsync();
        if (colors == null || !colors.Any())
        {
            return;
        }
        _colors = new SortedDictionary<int, Color>(colors
            .Select(x => new KeyValuePair<int, Color>(x.Age, new Color(x.Red, x.Green, x.Blue, x.Alpha)))
            .ToDictionary(x => x.Key, x=> x.Value));
        BuildColorsCache();
    }
    
    private static async Task SaveSetting(string field, string value) =>
        await _database.SaveSettingAsync(new SettingsEntity
        {
            FieldName = field,
            Value = value
        });

    private static async Task SaveColorSetting(int age, Color color) =>
        await _database.SaveColorAsync(new ColorAgeEntity
        {
            Age = age,
            Alpha = color.Alpha,
            Red = color.Red,
            Green = color.Green,
            Blue = color.Blue
        });

    private static async Task SaveColorSetting(int age) => await _database.RemoveColorAsync(age);

    private static async void SetRulestring(string value, bool bMode, bool save)
    {
        var parsed = value.ParseArgs();
        if (bMode)
        {
            await SetRulestring(parsed, null, save);
        }
        else
        {
            await SetRulestring(null, parsed, save);
        }
    }
    
    private static void BuildColorsCache()
    {
        _keys = _colors.Keys.ToList();
        _cachedColors = new Dictionary<int, Color>
        {
            [0] = new(255, 255, 255, 255)
        };
        var keys = _colors.Keys.ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            _cachedColors.Add(key, _colors[key]);
            if (i + 1 >= keys.Count)
            {
                continue;
            }

            var j = key + 1;
            var nextValue = keys[i + 1];
            while (j  < nextValue)
            {
                _cachedColors.Add(j, new Color(
                    (_colors[key].Red + _colors[nextValue].Red) / 2,
                    (_colors[key].Green + _colors[nextValue].Green) / 2,
                    (_colors[key].Blue + _colors[nextValue].Blue) / 2));
                j++;
            }
        }
    }

    private static async Task HandleMiscSettingsLoaded()
    {
        foreach (var value in Enum.GetValues(typeof(MiscSettingsBitflagEnum)))
        {
            await HandleMiscSettings((MiscSettingsBitflagEnum)value, _miscSettings.HasFlag((int)value), false);
        }
    }
    
    private static async Task HandleMiscSettings(MiscSettingsBitflagEnum bitflag, bool value, bool save = true)
    {
        switch (bitflag)
        {
            case MiscSettingsBitflagEnum.None:
                return;
            case MiscSettingsBitflagEnum.ManualNextButtonShown:
                _mainPage.ToggleNextButton(value);
                break;
            case MiscSettingsBitflagEnum.ScreenshotButtonShown:
                _mainPage.ToggleScreenshotButton(value);
                break;
        }
        
        if(save) await SaveSetting(Constants.FieldMiscSettings, _miscSettings.ToString());
    }
}