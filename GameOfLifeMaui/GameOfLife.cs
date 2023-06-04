using GameOfLifeMaui.Models;

namespace GameOfLifeMaui;

public sealed class Game
{
    public int MaxX { get; private set; }

    public int MaxY { get; private set; }

    private Cell[,] _generation;
    
    private int _cellSize;
    
    private int _xMargin;
    
    private int _yMargin;

    private const int CellSpacing = 2;
    
    public Game()
    {
        _generation = new Cell[MaxX, MaxY];
        _cellSize = SettingsManager.CurrentCellSize;
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
            if(x + i < 0 || x + i >= MaxX)
            {
                continue;
            }
            
            for (var j = -1; j <= 1; j++)
            {
                if (y + j < 0 || y + j >= MaxY)
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
        for (var i = 0; i < MaxX; i++)
        {
            for (var j = 0; j < MaxY; j++)
            {
                _generation[i, j].SetNextState(random.Next(0, 101) > 70);
                _generation[i, j].SetCurrentState();
            }
        }
    }
    
    public void Seed() => Seed(null);

    public void Clear()
    {
        for (var i = 0; i < MaxX; i++)
        {
            for (var j = 0; j < MaxY; j++)
            {
                _generation[i, j].SetNextState(false);
                _generation[i, j].SetCurrentState();
            }
        }
    }

    public async Task ChangeCellSize(int newSize)
    {
        if (_cellSize == newSize) return;

        _cellSize = newSize;
        await CalculateNewCellNumber();
        DrawNext();
    }
    
    public Task CalculateNewCellNumber(AbsoluteLayout layout = null)
    {
        if (layout == null)
        {
            if (_generation.Length == 0) return Task.CompletedTask;
            layout = _generation[0, 0]?.Parent as AbsoluteLayout;
        }
        
        var newY = (int)(layout!.Height / _cellSize);
        var newX = (int)(layout.Width / _cellSize);
        
        _xMargin = (int)((layout.Width - newX * _cellSize) / 2);
        _yMargin = (int)((layout.Height - newY * _cellSize) / 2);

        if (newY > 0 && newX > 0)
        {
            UpdateCellNumber(newX, newY, layout);
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
        else if (xValue >= MaxX)
        {
            xValue = MaxX - 1;
        }

        if (yValue < 0)
        {
            yValue = 0;
        }
        else if (yValue >= MaxY)
        {
            yValue = MaxY - 1;
        }

        return _generation[xValue, yValue];
    }

    public List<CellDto> PrepareForSave()
    {
        var result = new List<CellDto>();
        for (var i = 0; i < MaxX; i++)
        {
            for (var j = 0; j < MaxY; j++)
            {
                if (_generation[i, j].IsAlive)
                {
                    result.Add(new CellDto
                    {
                        X = i,
                        Y = j,
                        Age = _generation[i, j].Age
                    });
                }
            }
        }

        return result;
    }
    
    public void ReadFromSave(List<CellDto> data)
    {
        foreach (var cell in data)
        {
            if(cell.X >= MaxX || cell.Y >= MaxY) continue;
            _generation[cell.X, cell.Y].SetNextState(true);
            _generation[cell.X, cell.Y].SetCurrentState();
            _generation[cell.X, cell.Y].Age = cell.Age;
        }
    }

    public void SetCellState(int x, int y, bool alive)
    {
        if (alive == _generation[x, y].IsAlive) return;
        _generation[x, y].SetNextState(alive);
        _generation[x, y].SetCurrentState();
    }

    private void UpdateCellNumber(int newX, int newY, AbsoluteLayout layout)
    {
        if (newX < 0 || newY < 0 || newX == MaxX && newY == MaxY)
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
                if (j < MaxY && i < MaxX)
                {
                    newBoard[i, j] = _generation[i, j];
                }
                //out of bounds
                else
                {
                    newBoard[i, j] = new Cell(i, j, false, this);
                    layout.Add(newBoard[i, j]);
                }
            }

            if (newY >= MaxY || i >= MaxX)
            {
                continue;
            }
            
            for(var j = newY; j < MaxY; j++)
            {
                layout.Remove(_generation[i, j]);
            }
        }
        if (newX < MaxX)
        {
            for(var i = newX; i < MaxX; i++)
            {
                for(var j = 0; j < MaxY; j++)
                {
                    layout.Remove(_generation[i, j]);
                }
            }
        }

        foreach (var cell in newBoard)
        {
            layout.SetLayoutBounds(cell, new Rect(
                // ReSharper disable PossibleLossOfFraction
                cell.IndexX * _cellSize + _xMargin + CellSpacing / 2, 
                cell.IndexY * _cellSize + _yMargin + CellSpacing / 2, 
                _cellSize - CellSpacing, 
                _cellSize - CellSpacing));
        }

        _generation = newBoard;
        MaxX = newX;
        MaxY = newY;
    }
    
    private void SetNextStateGlobal()
    {
        for (var i = 0; i < MaxX; i++)
        {
            for (var j = 0; j < MaxY; j++)
            {
                _generation[i, j].SetCurrentState();
            }
        }
    }

    private bool CreateNextGeneration()
    {
        var anyCellAlive = false;
        
        for (var i = 0; i < MaxX; i++)
        {
            for (var j = 0; j < MaxY; j++)
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