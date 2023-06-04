namespace GameOfLifeMaui.Fractals;

public sealed class SierpinskiTriangle : Fractal
{
    private readonly Game _game;

    private const int MinSize = 5;

    public SierpinskiTriangle(Game game)
    {
        _game = game;
    }


    public override void Act()
    {
        var x = _game.MaxX;
        var y = _game.MaxY;

        if (x < MinSize || y < MinSize) return;

        // x -= x - x % 2;
        // y -= y - y % 2;
        var length = Math.Min(x, y);
        // if (length == 0)
        // {
        //     length = Math.Min(_game.MaxX % 2 == 0 ? _game.MaxX - 1 : _game.MaxX,
        //         _game.MaxY % 2 == 0 ? _game.MaxY - 1 : _game.MaxY);
        // }
        
        Draw(0, 0, length, 0);
    }

    public override void SetParameters(int? width, int? height, int? steps)
    {
        var bigger = Math.Max(height ?? 0, width ?? 0);
        if (bigger >= MinSize)
        {
            if(bigger % 2 != 1) bigger -= 1;
            Width = bigger;
            Height = bigger;
        }

        if (steps > 0)
        {
            Steps = steps;
        }
    }

    private void Draw(int x, int y, int length, int step)
    {
        if (length < MinSize) return;

        if (length % 2 != 1) length -= 1;
        
        var level = 0;
        var maxLevel = length / 2 + 1;
        var topX = x;
        for (var i = x + length - 1; i > x; i--)
        {
            var startX = y + level;
            if (startX > length)
            {
                topX = i;
                break;
            }
            
            for(var j = startX; j < length - level; j++)
            {
                _game.SetCellState(j, i, !IsMiddleTriangle(j, startX, level, maxLevel, length - level));
            }

            level++;
        }

        if (step + 1 >= Steps) return;
        
        var nextLength = length / 3;

        //Upper triangle
        Draw(topX, y, nextLength, step + 1);
        //Left triangle
        Draw(topX , y + length / 2, nextLength, step + 1);
        //Right triangle
        Draw(topX + length / 2, y, nextLength, step + 1);
    }

    private static bool IsMiddleTriangle(int x, int startX, int level, int maxLevel, int length)
    {
        var fullLength = 1 + 2 * level;
        if(fullLength >= length) return false;
        var pos = x - startX;
        var currentLevel = maxLevel - level - 1;
        return pos <= currentLevel + level && pos >= currentLevel - level;
    }
}