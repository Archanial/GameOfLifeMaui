using GameOfLifeMaui.ViewModels.Pages;
using View = Microsoft.Maui.Controls.View;

namespace GameOfLifeMaui;

public static class PanGestureHandler
{
    private static Game _game;
    
    private static MainPage _mainPage;

    private static AbsoluteLayout _layout;
    
    private static double _lastPanX;

    private static double _lastPanY;

    private static Cell _lastCell;
    
    private const double Tolerance = 0.1;

    private static List<Cell> _frozenCells = new();

    public static void Initialize(Game game, MainPage mainPage)
    {
        _game = game;
        _mainPage = mainPage;
        _layout = mainPage.GameLayout;
    }
    
    public static void OnPanUpdated(object sender, PanUpdatedEventArgs e)
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

        if (view.Parent is not View parentLayout)
        {
            parentLayout = view;
        }
        
        var currentX = view.X + e.TotalX;
        var currentY = view.Y + e.TotalY;
        if (currentX > parentLayout.Width)
        {
            currentX = parentLayout.Width;
        }

        if (currentY > parentLayout.Height)
        {
            currentY = parentLayout.Height;
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