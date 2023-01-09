namespace GameOfLifeMaui;

[Obsolete]
public sealed class CellOld : BoxView
{
    public bool IsAlive;

    private bool _nextState;

    private readonly Game _game;

    private int _age;

    public int IndexX { get; }

    public int IndexY { get; }

    private bool _isFrozen;
    
    public CellOld(int x, int y, bool alive, Game game)
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
        GestureRecognizers.Add(tapGesture);
        GestureRecognizers.Add(panGesture);

        //Margin = -1;
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
            BackgroundColor = GetColor();
        }
        _nextState = alive;
    }

    public void Freeze()
    {
        if (_isFrozen)
        {
            return;
        }
        
        BackgroundColor = new Color(128, 128, 128);
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
    
    private Color GetColor()
    {
        //TODO: think of a good colors
        var value = Math.Max(255 - _age * 255, 0);
        return Color.FromRgb(value, value, value);
    }
}