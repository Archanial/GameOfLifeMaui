namespace GameOfLifeMaui;

public sealed class Game
{
    private int _maxX;

    private int _maxY;

    private Cell[,] _generation;
    
    private int _cellSize = 30;
    
    private int _xMargin;
    
    private int _yMargin;

    public readonly AbsoluteLayout Layout;
    
    private const int CellSpacing = 2;
    
    public Game(AbsoluteLayout layout)
    {
        _generation = new Cell[_maxX, _maxY];
        Layout = layout;
    }
    
    public bool DrawNext()
    {
        var result = CreateNextGeneration();
        SetNextStateGlobal();
        return result;
    }
    
    public int GetLivingCells(int x, int y)
    {
        var aliveCells = 0;
        for (var i = -1; i <= 1; i++)
        {
            if(x + i < 0 || x + i >= _maxX)
            {
                continue;
            }
            
            for (var j = -1; j <= 1; j++)
            {
                if (y + j < 0 || y + j >= _maxY)
                {
                    continue;
                }

                aliveCells += _generation[x + i, y + j].IsAlive ? 1 : 0;
            }
        }

        return aliveCells - (_generation[x, y].IsAlive ? 1 : 0);
    }
    
    public void Seed(int? seed)
    {
        Clear();
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        for (var i = 0; i < _maxX; i++)
        {
            for (var j = 0; j < _maxY; j++)
            {
                _generation[i, j].SetNextState(random.Next(0, 101) > 70);
                _generation[i, j].SetCurrentState();
            }
        }
    }
    
    public void Seed() => Seed(null);

    public void Clear()
    {
        for (var i = 0; i < _maxX; i++)
        {
            for (var j = 0; j < _maxY; j++)
            {
                _generation[i, j].SetNextState(false);
                _generation[i, j].SetCurrentState();
            }
        }
    }

    public async Task ChangeCellSize(int newSize)
    {
        _cellSize = newSize;
        await CalculateNewCellNumber(Layout);
        DrawNext();
    }
    
    public Task CalculateNewCellNumber(Layout layout)
    {
        var newY = (int)(layout.Height / _cellSize);
        var newX = (int)(layout.Width / _cellSize);
        
        _xMargin = (int)((layout.Width - newX * _cellSize) / 2);
        _yMargin = (int)((layout.Height - newY * _cellSize) / 2);

        if (newY > 0 && newX > 0)
        {
            UpdateCellNumber(newX, newY);
        }

        return Task.CompletedTask;
    }

    public void UpdateColors()
    {
        foreach (var cell in _generation)
        {
            cell.UpdateColor();
        }
    }
    
    public Cell GetNearestChild(double x, double y)
    {
        var xValue = ((int)Math.Round(x) - CellSpacing/2 - _xMargin) / _cellSize;
        var yValue = ((int)Math.Round(y) - CellSpacing/2 - _yMargin) / _cellSize;

        if (xValue < 0)
        {
            xValue = 0;
        }
        else if (xValue >= _maxX)
        {
            xValue = _maxX - 1;
        }

        if (yValue < 0)
        {
            yValue = 0;
        }
        else if (yValue >= _maxY)
        {
            yValue = _maxY - 1;
        }

        return _generation[xValue, yValue];
    }
    
    private void UpdateCellNumber(int newX, int newY)
    {
        if (newX < 0 || newY < 0 || newX == _maxX && newY == _maxY)
        {
            return;
        }
        
        var newBoard = new Cell[newX, newY];
        
        for (var i = 0; i < newX; i++)
        {
            //we are in bounds
            for (var j = 0; j < newY; j++)
            {
                //we are in bounds
                if (j < _maxY && i < _maxX)
                {
                    newBoard[i, j] = _generation[i, j];
                }
                //out of bounds
                else
                {
                    newBoard[i, j] = new Cell(i, j, false, this);
                    Layout.Add(newBoard[i, j]);
                }
            }

            if (newY >= _maxY || i >= _maxX)
            {
                continue;
            }
            
            for(var j = newY; j < _maxY; j++)
            {
                Layout.Remove(_generation[i, j]);
            }
        }
        if (newX < _maxX)
        {
            for(var i = newX; i < _maxX; i++)
            {
                for(var j = 0; j < _maxY; j++)
                {
                    Layout.Remove(_generation[i, j]);
                }
            }
        }

        foreach (var cell in newBoard)
        {
            Layout.SetLayoutBounds(cell, new Rect(
                cell.IndexX * _cellSize + _xMargin + CellSpacing / 2, 
                cell.IndexY * _cellSize + _yMargin + CellSpacing / 2, 
                _cellSize - CellSpacing, 
                _cellSize - CellSpacing));
        }

        _generation = newBoard;
        _maxX = newX;
        _maxY = newY;
    }

    private void SetNextStateGlobal()
    {
        for (var i = 0; i < _maxX; i++)
        {
            for (var j = 0; j < _maxY; j++)
            {
                _generation[i, j].SetCurrentState();
            }
        }
    }

    private bool CreateNextGeneration()
    {
        var anyCellAlive = false;
        
        for (var i = 0; i < _maxX; i++)
        {
            for (var j = 0; j < _maxY; j++)
            {
                if (_generation[i, j].ProcessLife())
                {
                    anyCellAlive = true;
                }
            }
        }

        return anyCellAlive;
    }
}