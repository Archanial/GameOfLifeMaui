namespace GameOfLifeMaui;

public static class Helper
{
    public static string ToColorString(this Color value) =>
        $"R:{(int)(value.Red * 255f)} G:{(int)(value.Green * 255f)} B:{(int)(value.Blue * 255f)}";
}