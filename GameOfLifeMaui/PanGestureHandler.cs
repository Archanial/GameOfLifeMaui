using AView = Android.Views.View;
using View = Microsoft.Maui.Controls.View;

namespace GameOfLifeMaui;

public sealed class PanGestureHandler
{
    private static PanGestureHandler _instance;
    
    private readonly Game _game;

    private readonly AbsoluteLayout _layout;
    
    private double _lastPanX;

    private double _lastPanY;

    private Cell _lastCell;
    
    private const double Tolerance = 0.1;

    private List<Cell> _frozenCells = new();

    private PanGestureHandler(Game game)
    {
        _game = game;
        _layout = game.Layout;
    }
    
    public static PanGestureHandler GetInstance(Game game)
    {
        return _instance ??= new PanGestureHandler(game);
    }

    public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType is GestureStatus.Canceled or GestureStatus.Completed)
        {
            _lastCell = null;
            _lastPanX = 0;
            _lastPanY = 0;

            foreach (var frozenCell in _frozenCells)
            {
                frozenCell.UnFreeze();
            }
            _frozenCells = new List<Cell>();
            return;
        }
        
        if (sender is not View view)
        {
            return;
        }
        
        //Let's assume we are in the middle of the cell
        var currentX = view.X + view.Height/2 + e.TotalX;
        var currentY = view.Y + view.Width/2 + e.TotalY;
        if (currentX > _layout.Width)
        {
            currentX = _layout.Width;
        }

        if (currentY > _layout.Height)
        {
            currentY = _layout.Height;
        }

        if (Math.Abs(_lastPanX - currentX) < Tolerance || Math.Abs(_lastPanY - currentY) < Tolerance)
        {
            return;
        }

        _lastPanX = currentX;
        _lastPanY = currentY;


        var cell = _game.GetNearestChild(currentX, currentY);
        if (cell == _lastCell)
        {
            return;
        }
        
        _frozenCells.Add(cell);
        cell.Freeze();
        _lastCell = cell;
    }
}