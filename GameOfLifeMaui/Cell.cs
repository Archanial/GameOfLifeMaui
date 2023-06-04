namespace GameOfLifeMaui;

public sealed class Cell : Frame
{
    public bool IsAlive;
    
    public int Age { get; set; }

    private bool _nextState;

    private readonly Game _game;

    public int IndexX { get; }

    public int IndexY { get; }

    private bool _isFrozen;
    
    public Cell(int x, int y, bool alive, Game game)
    {
        IndexX = x;
        IndexY = y;
        _game = game;
        IsAlive = alive;
        _nextState = alive;

        var tapGesture = new TapGestureRecognizer();
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += PanGestureHandler.OnPanUpdated;
        tapGesture.Tapped += (_, _) => OnClick();
        GestureRecognizers.Add(tapGesture);
        GestureRecognizers.Add(panGesture);
        BackgroundColor = new Color(81, 43, 212); //same as background
        Content = new BoxView();
        Margin = -1;
        Padding = new Thickness(1, 1, 1, 1);
        HasShadow = false;
        IsClippedToBounds = true;
        CornerRadius = 0;
    }

    public bool ProcessLife()
    {
        var livingCellsNearby = _game.GetLivingCells(IndexX, IndexY);
        if (IsAlive)
        {
            SetNextState(IndexFound(livingCellsNearby, SettingsManager.SArg));
            return IsAlive;
        }

        SetNextState(IndexFound(livingCellsNearby, SettingsManager.BArg));
        return IsAlive;
    }

    public void SetCurrentState()
    {
        IsAlive = _nextState;
    }

    public void SetNextState(bool alive, int? age = null)
    {
        if (alive)
        {
            if (age.HasValue)
            {
                Age = age.Value;
            }
            else
            {
                Age++;
            }
        }
        else
        {
            Age = 0;
        }

        if (!_isFrozen)
        {
            UpdateColor();
        }
        _nextState = alive;
    }

    public void UpdateColor() => ((BoxView)Content).Color = SettingsManager.GetColor(Age);

    public void Freeze()
    {
        if (_isFrozen)
        {
            return;
        }
        
        ((BoxView)Content).Color = new Color(128, 128, 128);
        _isFrozen = true;
    }
    
    public void UnFreeze()
    {
        if (!_isFrozen)
        {
            return;
        }
        
        _isFrozen = false;
        OnClick(true);
    }

    public void OnClick(bool? forcedAlive = null)
    {
        Age = SettingsManager.TappedAge;
        SetNextState(forcedAlive ?? !IsAlive);
        SetCurrentState();
    }

    private static bool IndexFound(int value, int[] array)
    {
        var index = Array.BinarySearch(array, value);
        return index >= 0 && index < array.Length;
    }
}