namespace GameOfLifeMaui;

public sealed class Cell : Frame
{
    public bool IsAlive;

    private bool _nextState;

    private readonly Game _game;

    private int _age;

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
        panGesture.PanUpdated += PanGestureHandler.GetInstance(_game).OnPanUpdated;
        tapGesture.Tapped += (_, _) => OnClick();
        //_internalImage.GestureRecognizers.Add(tapGesture);
        //_internalImage.GestureRecognizers.Add(panGesture);
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
            SetNextState(livingCellsNearby is 2 or 3);
            return IsAlive;
        }

        SetNextState(livingCellsNearby == 3);

        return IsAlive;
    }

    public void SetCurrentState()
    {
        IsAlive = _nextState;
    }

    public void SetNextState(bool alive)
    {
        if (alive)
        {
            _age++;
        }
        else
        {
            _age = 0;
        }

        if (!_isFrozen)
        {
            ((BoxView)Content).Color = GetColor();
        }
        _nextState = alive;
    }

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
        _age = 10;
        SetNextState(forcedAlive ?? !IsAlive);
        SetCurrentState();
    }
    
    private Color GetColor() =>
        _age switch
        {
            0 => Color.FromRgb(255, 255, 255),
            1 => Color.FromRgb(165, 245, 122),
            2 => Color.FromRgb(145, 237, 95),
            3 => Color.FromRgb(120, 240, 55),
            4 => Color.FromRgb(87, 199, 26),
            5 => Color.FromRgb(62, 145, 16),
            6 => Color.FromRgb(43, 105, 9),
            7 => Color.FromRgb(32, 79, 6),
            8 => Color.FromRgb(21, 54, 3),
            _ => Color.FromRgb(0, 0, 0)
        };
}