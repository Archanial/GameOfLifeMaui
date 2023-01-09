namespace GameOfLifeMaui;

public static class SettingsManager
{
    public static Game Game { private get; set; }

    public static int CurrentCellSize { get; private set; } = 30;

    public const int MinCellSize = 12;

    public static async Task ChangeCellSize(int newSize)
    {
        CurrentCellSize = newSize;
        await Game.ChangeCellSize(newSize);
    }
}