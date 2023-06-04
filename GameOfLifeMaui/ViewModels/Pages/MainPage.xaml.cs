using System.Text;
using System.Text.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using GameOfLifeMaui.Models;

// ReSharper disable MethodHasAsyncOverload

namespace GameOfLifeMaui.ViewModels.Pages;

public sealed partial class MainPage
{
    private readonly Game _game;
    
    private readonly IDispatcherTimer _timer;

    private int _counter;
    
    private readonly Button _nextButton;
    
    private readonly Button _screenshotButton;

    private bool _isCounterTickRunning;
    
    private readonly IFileSaver _fileSaver;

    private readonly IFolderPicker _folderPicker;

    private readonly IFilePicker _filePicker;
    
    public MainPage(IFileSaver saver, IFolderPicker folderPicker, IFilePicker filePicker, Game game)
    {
        InitializeComponent();
        
        _game = game;
        _fileSaver = saver;
        _folderPicker = folderPicker;
        _filePicker = filePicker;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.IsRepeating = true;
        _timer.Tick += OnTimerTick;
        
        _nextButton = new Button
        {
            Text = "Next",
#pragma warning disable CS0618
            HorizontalOptions = LayoutOptions.FillAndExpand,
#pragma warning restore CS0618
            Padding = 0
        };
        _nextButton.Clicked += Next;
        _nextButton.IsVisible = false;
        _screenshotButton = new Button
        {
            Text = "Screenshot",
#pragma warning disable CS0618
            HorizontalOptions = LayoutOptions.FillAndExpand,
#pragma warning restore CS0618
            Padding = 0
        };
        _screenshotButton.Clicked += Screenshot;
        _screenshotButton.IsVisible = false;
        
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTapped;
        GameLayout.GestureRecognizers.Add(tapGesture);
    }

    public void ToggleNextButton(bool show)
    {
        if (show == _nextButton.IsVisible) return;
        if (show)
        {
            ShowButton(_nextButton, 2);
        }
        else
        {
            HideButton(_nextButton);
        }
    }
        
    public void ToggleScreenshotButton(bool show)
    {
        if (show == _screenshotButton.IsVisible) return;
        if (show)
        {
            ShowButton(_screenshotButton, ButtonHolder.Children.Count - 1);
        }
        else
        {
            HideButton(_screenshotButton);
        }
    }
    
    public async Task SaveLayoutToFile()
    {
        try
        {
            var path = await _folderPicker.PickAsync(default);
            if (!path.IsSuccessful || path.Folder == null) return;
            var preparedTable = _game.PrepareForSave();
            var json = JsonSerializer.Serialize(preparedTable);
            using var memoryStream = new MemoryStream(Encoding.Default.GetBytes(json));
            await _fileSaver.SaveAsync(path.Folder.Path, $"Board{DateTime.Now}.json", memoryStream, default);
        }
        catch (Exception e)
        {
            await Toast.Make(e.Message, ToastDuration.Long).Show();
            return;
        }

        await Toast.Make("Save to file!").Show();
    }
    
    public async Task ReadLayoutFromFile()
    {
        try
        {
            var customFiletype = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.macOS, new[] { "public.json" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.Tizen, new[] { "*/*" } }
            });
            var file = await _filePicker.PickAsync(new PickOptions
            {
                FileTypes = customFiletype,
                PickerTitle = "Select layout to restore"
            });
            if (file == null) return;

            var stream = await file.OpenReadAsync();
            var json = JsonSerializer.Deserialize<List<CellDto>>(stream);
            if (json == null || !json.Any()) return;
            _game.ReadFromSave(json);
        }
        catch (Exception e)
        {
            await Toast.Make(e.Message, ToastDuration.Long).Show();
            return;
        }

        await Toast.Make("Save to file!").Show();
    }

    private void ShowButton(Button button, int position)
    {
        ButtonHolder.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
        foreach (var child in ButtonHolder.Children)
        {
            var column = ButtonHolder.GetColumn(child);
            if(column < position) continue;
            ButtonHolder.SetColumn(child, column + 1);
        }
        ButtonHolder.Add(button);
        ButtonHolder.SetColumn(button, position);
        button.IsVisible = true;
    }
    
    private void HideButton(Button button)
    {
        var position = ButtonHolder.GetColumn(button);
        ButtonHolder.Remove(button);
        ButtonHolder.ColumnDefinitions.RemoveAt(0);
        foreach (var child in ButtonHolder.Children)
        {
            var column = ButtonHolder.GetColumn(child);
            if(column < position) continue;
            ButtonHolder.SetColumn(child, column - 1);
        }
        button.IsVisible = false;
    }

    //TODO: use IFileSaver class https://github.com/CommunityToolkit/Maui/pull/699
    private async void Screenshot(object sender, EventArgs eventArgs)
    {
        try
        {
            var screenshot = await GameLayout.CaptureAsync();
            if (screenshot == null) return;
            
            using var memoryStream = new MemoryStream();
            await screenshot.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
#if WINDOWS
            await File.WriteAllBytesAsync($"GameOfLife{DateTime.Now}.png", memoryStream.ToArray());
#elif ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            var resolver = Platform.CurrentActivity!.ContentResolver;
            Android.Content.ContentValues contentValues = new();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, $"GameOfLife{DateTime.Now}.png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/Camera");
            var imageUri = resolver!.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri!, 
                contentValues);
            if(imageUri == null) return;
            var os = resolver.OpenOutputStream(imageUri);
            var bitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(memoryStream);
            bitmap!.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 80, os);
            os!.Flush();
            os.Close();
        }
        else
        {
            var storagePath = 
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            var path = Path.Combine(storagePath!.ToString(), $"GameOfLife{DateTime.Now}.png");
            File.WriteAllBytes(path, memoryStream.ToArray());
            var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
            Platform.CurrentActivity!.SendBroadcast(mediaScanIntent);
        }

#elif IOS || MACCATALYST
        var image = new UIKit.UIImage(Foundation.NSData.FromArray(memoryStream.ToArray()));
        image.SaveToPhotosAlbum((_, _) =>{});
#endif
        }
        catch (Exception e)
        {
            await Toast.Make(e.Message, ToastDuration.Long).Show();
            return;
        }

        await Toast.Make("Saved screenshot!").Show();
    }

    private async void OnLayoutSizeChanged(object sender, EventArgs args)
    {
        await _game.CalculateNewCellNumber(GameLayout);
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
        if(_isCounterTickRunning) CleanUp();
    }

    private void OnTimerTick(object sender, EventArgs eventArgs)
    {
        if (!_game.DrawNext())
        {
            StopRunning();
        }
    }
    
    private void OnCountedTimerTick(object sender, EventArgs eventArgs)
    {
        if (_counter >= SettingsManager.NextButtonFrames)
        {
            CleanUp();
            return;
        }

        _counter++;
        if (!_game.DrawNext())
        {
            StopRunning();
        }
    }
    
    private void Next(object sender, EventArgs args)
    {
        if (_isCounterTickRunning) return;
        
        StopRunning();
        if (SettingsManager.NextButtonFrames > 1)
        {
            _isCounterTickRunning = true;
            _timer.Tick -= OnTimerTick;
            _timer.Tick += OnCountedTimerTick;
            RunButton.Text = "Stop";
            _timer.Start();
        }
        else
        {
            _game.DrawNext();
        }
    }

    private void CleanUp()
    {
        _isCounterTickRunning = false;
        _timer.Stop();
        _timer.Tick -= OnCountedTimerTick;
        _timer.Tick += OnTimerTick;
        RunButton.Text = "Start";
        _counter = 0;
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
        var position = args.GetPosition(GameLayout);
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
        var page = new SettingsPage();
        await Navigation.PushAsync(page);
    }
}