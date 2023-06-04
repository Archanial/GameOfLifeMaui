namespace GameOfLifeMaui.Fractals;

public sealed class SierpinskiCarpet : Fractal
{
    private readonly Game _game;

    private const int MinSize = 3;

    public SierpinskiCarpet(Game game)
    {
        _game = game;
    }
    
    public override void Act()
    {
        var x = _game.MaxX;
        var y = _game.MaxY;

        if (x < MinSize || y < MinSize) return;

        x -= x % 3;
        y -= y % 3;
        DrawCarpet(0, 0, Math.Min(x, y), 0);
    }

    public override void SetParameters(int? width, int? height, int? steps)
    {
        if (width >= MinSize)
        {
            if(width % 3 != 0) width -= width % 3;
            Width = width;
            Height = width;
        }

        if (steps > 0)
        {
            Steps = steps;
        }
    }

    private void DrawCarpet(int x, int y, int length, int step)
    {
        if (length < MinSize || length % 3 != 0) return;
        if (Width.HasValue)
        {
            length = int.Min(length, Width.Value);
        }

        for (var i = x; i < x + length; i++)
        {
            for (var j = y; j < y + length; j++)
            {
                _game.SetCellState(i, j, !InMiddle(i, j, x, y, length));
            }
        }

        step += 1;
        if(step + 1 > Steps) return;
        var nextLength = length / 3;
        for (var i = 0; i < 3; i++)
        {
            //Upper row
            DrawCarpet(x + i * nextLength, y, nextLength, step);
            //Bottom row
            DrawCarpet(x + i * nextLength, y + 2 * nextLength, nextLength, step);
            //Middle row
            if (i != 1) DrawCarpet(x + i * nextLength, y + nextLength, nextLength, step);
        }
    }

    private static bool InMiddle(int x, int y, int startX, int startY, int length)
    {
        if (x == startX || y == startY) return false;

        var increment = length / 3;
        var middle = (double)length / 2;
        var half = increment / 2;
        var leftBound = middle - half;
        var rightBound = middle + half;

        var xPos = x - startX;
        if(xPos < leftBound || xPos >= rightBound)
        {
            return false;
        }
        
        var yPos = y - startY;
        if(yPos < leftBound || yPos >= rightBound)
        {
            return false;
        }

        return true;
    }
}