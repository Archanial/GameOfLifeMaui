namespace GameOfLifeMaui.Models;

public sealed record CellDto
{
    public required int X { get; init; }
    
    public required int Y { get; init; }
    
    public required int Age { get; init; }
}