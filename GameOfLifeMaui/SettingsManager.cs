using GameOfLifeMaui.Entities;

namespace GameOfLifeMaui;

public static class SettingsManager
{
    public static Game Game { private get; set; }

    public static int CurrentCellSize { get; private set; } = 30;

    public const int MinCellSize = 12;
    
    public const int MaxCellSize = 50;

    public static int TappedAge { get; private set; }
    
    public static int[] BArg { get; private set; }
    
    public static int[] SArg { get; private set; }
    
    public static IDatabase Database { private get; set; }
    
    private static SortedDictionary<int, Color> _colors = new();

    private static List<int> _keys = new();

    private static Dictionary<int, Color> _cachedColors;

    static SettingsManager()
    {
        TappedAge = 10;
        BArg = new[] { 3 };
        SArg = new[] { 2, 3 };
        BuildColorsCache();
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

    public static async void LoadSettings()
    {
        var settings = await Database.GetSettingsAsync();
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
            }
        }
        
        var colors = await Database.GetColorsAsync();
        if (colors == null || !colors.Any())
        {
            return;
        }
        _colors = new SortedDictionary<int, Color>(colors
            .Select(x => new KeyValuePair<int, Color>(x.Age, new Color(x.Red, x.Green, x.Blue, x.Alpha)))
            .ToDictionary(x => x.Key, x=> x.Value));
        BuildColorsCache();
        Game.UpdateColors();
    }
    
    private static async Task SaveSetting(string field, string value)
    {
        await Database.SaveSettingAsync(new SettingsEntity
        {
            FieldName = field,
            Value = value
        });
    }
    
    private static async Task SaveColorSetting(int age, Color color)
    {
        await Database.SaveColorAsync(new ColorAgeEntity
        {
            Age = age,
            Alpha = color.Alpha,
            Red = color.Red,
            Green = color.Green,
            Blue = color.Blue
        });
    }
    
    private static async Task SaveColorSetting(int age) => await Database.RemoveColorAsync(age);

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

    private static string ArrayToString(this int[] array)
    {
        if (array.Length == 0)
        {
            return "";
        }
        
        var arrayString = array[0].ToString();

        for (var i = 1; i < array.Length; i++)
        {
            arrayString += $",{array[i]}";
        }

        return arrayString;
    }

    private static int[] ParseArgs(this string s)
    {
        var sArray = s.Split(",");

        return Array.ConvertAll(sArray, int.Parse);
    }
}