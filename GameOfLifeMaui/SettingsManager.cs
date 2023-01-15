namespace GameOfLifeMaui;

public static class SettingsManager
{
    public static Game Game { private get; set; }

    public static int CurrentCellSize { get; private set; } = 30;

    public const int MinCellSize = 12;
    
    public const int MaxCellSize = 50;

    public static int TappedAge { get; set; }
    
    public static int[] BArg { get; private set; }
    
    public static int[] SArg { get; private set; }
    
    private static readonly SortedDictionary<int, Color> Colors = new();

    private static List<int> _keys = new();

    private static Dictionary<int, Color> _cachedColors;
    
    static SettingsManager()
    {
        TappedAge = 10;
        BArg = new[] { 3 };
        SArg = new[] { 2, 3 };
        BuildColorsCache();
    }
    
    public static async Task ChangeCellSize(int newSize)
    {
        CurrentCellSize = newSize;
        await Game.ChangeCellSize(newSize);
    }

    public static IEnumerable<KeyValuePair<int, Color>> GetColors() => Colors.ToList();

    public static void AddOrUpdateColor(int age, Color color)
    {
        if (Colors.ContainsKey(age))
        {
            if (Equals(Colors[age], color))
            {
                return;
            }
            Colors[age] = color;
        }
        else
        {
            Colors.Add(age, color);
        }

        BuildColorsCache();
        Game.UpdateColors();
    }
    
    public static void TryRemoveColor(int age)
    {
        Colors.Remove(age);
        BuildColorsCache();
        Game.UpdateColors();
    }

    public static string GetRuleString => $"B{string.Join("", BArg)}/S{string.Join("", SArg)}";

    public static void SetRulestring(int[] b, int[] s)
    {
        if (b.Any())
        {
            BArg = b;
        }

        if (s.Any())
        {
            SArg = s;
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

    private static void BuildColorsCache()
    {
        _keys = Colors.Keys.ToList();
        _cachedColors = new Dictionary<int, Color>
        {
            [0] = new(255, 255, 255, 255)
        };
        var keys = Colors.Keys.ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            _cachedColors.Add(key, Colors[key]);
            if (i + 1 >= keys.Count)
            {
                continue;
            }

            var j = key + 1;
            var nextValue = keys[i + 1];
            while (j  < nextValue)
            {
                _cachedColors.Add(j, new Color(
                    (Colors[key].Red + Colors[nextValue].Red) / 2,
                    (Colors[key].Green + Colors[nextValue].Green) / 2,
                    (Colors[key].Blue + Colors[nextValue].Blue) / 2));
                j++;
            }
        }
    }
}