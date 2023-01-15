namespace GameOfLifeMaui;

public partial class MainPage
{
    private readonly Game _game;
    
    private readonly IDispatcherTimer _timer;
    
    public MainPage()
    {
        InitializeComponent();
        
        _game = new Game(AbsoluteLayout);
        SettingsManager.Game = _game;
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.IsRepeating = true;
        _timer.Tick += (_, _) =>
        {
            OnTimerTick();
        };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTapped;
        // var panGesture = new PanGestureRecognizer();
        // panGesture.PanUpdated += PanGestureHandler.GetInstance(_game).OnPanUpdated;
        // AbsoluteLayout.GestureRecognizers.Add(panGesture);
        AbsoluteLayout.GestureRecognizers.Add(tapGesture);
    }

    private async void OnLayoutSizeChanged(object sender, EventArgs args)
    {
        if(sender is not Layout layout)
        {
            return;
        }
        
        await _game.CalculateNewCellNumber(layout);
        _game.DrawNext();
    }

    private void OnRunButtonClicked(object sender, EventArgs args)
    {
        if (!_timer.IsRunning)
        {
            RunButton.Text = "Pause";
            _timer.Start();
        }
        else
        {
            StopRunning();
        }
    }

    private void StopRunning()
    {
        RunButton.Text = "Start";
        _timer.Stop();
    }

    private void OnTimerTick()
    {
        if (!_game.DrawNext())
        {
            StopRunning();
        }
    }

    private void OnClearButtonClicked(object sender, EventArgs args)
    {
        if (_timer.IsRunning)
        {
            StopRunning();
        }
        _game.Clear();
    }

    private async void OnSeedButtonClicked(object sender, EventArgs args)
    {
        var result = await DisplayPromptAsync(
            "Seed number", 
            "Input seed number (or leave blank for random).", 
            keyboard : Keyboard.Numeric );

        if (result == null)
        {
            return;
        }
        
        if (int.TryParse(result, out var seed))
        {
            _game.Seed(seed);
            return;
        }
        
        _game.Seed();
    }
    
    private async void OnSpeedButtonClicked(object sender, EventArgs args)
    {
        var result = await DisplayPromptAsync(
            "Change speed", 
            "Input new speed (in ms).", 
            keyboard : Keyboard.Numeric );

        if (result == null)
        {
            return;
        }
        
        if (int.TryParse(result, out var speed))
        {
            if (speed < 1)
            {
                return;
            }

            _timer.Interval = TimeSpan.FromMilliseconds(speed);
            return;
        }
        
        _game.Seed();
    }
    
    private void OnTapped(object sender, TappedEventArgs args)
    {
        var position = args.GetPosition(AbsoluteLayout);
        if (position == null)
        {
            return;
        }

        var child = _game.GetNearestChild(position.Value.X, position.Value.Y);
        child.OnClick();
    }

    private async void OnSettingsButtonClicked(object sender, EventArgs eventArgs)
    {
        StopRunning();
        await Navigation.PushAsync(new SettingsPage()); 
    }
}