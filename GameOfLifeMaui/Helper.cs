namespace GameOfLifeMaui;

public static class Helper
{
    public static string ToColorString(this Color value) =>
        $"R:{(int)(value.Red * 255f)} G:{(int)(value.Green * 255f)} B:{(int)(value.Blue * 255f)}";

    public static int ToInt(this bool value) => value ? 1 : 0;
    
    public static bool HasFlag(this int value, int flag) => (value & flag) != 0;
    
    public static string ArrayToString(this int[] array)
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
    
    public static int[] ParseArgs(this string s)
    {
        var sArray = s.Split(",");

        return Array.ConvertAll(sArray, int.Parse);
    }
}