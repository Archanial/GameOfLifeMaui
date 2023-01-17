using CommunityToolkit.Maui.Views;
using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui.ViewModels;

public partial class ColorHolder
{
    private readonly int _age;
    
    private static string ImageString =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? "Images/x_symbol.svg"
            : "Images/x_symbol_light.svg";

    public ColorHolder(Color color, int age)
    {
        InitializeComponent();

        _age = age;
        BackgroundColor = new Color(0, 0, 0, 0);
        
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = GridLength.Auto },
                new() { Width = GridLength.Star },
                new() { Width = GridLength.Auto }
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new()
            },
            BackgroundColor = new Color(0, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        var internalGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new()
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new(),
                new()
            },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(5, 0, 0, 0)
        };

        Content = grid;
        grid.Add(new BoxView
        {
            WidthRequest = 50,
            HeightRequest = 50,
            Color = color,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start
        }, 0);
        grid.Add(internalGrid, 1);
        var deleteButton = new Button
        {
            ImageSource = ImageString,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = BackgroundColor,
            HeightRequest = 30,
            WidthRequest = 30
        };
        deleteButton.Clicked += DeleteButtonOnClicked;
        grid.Add(deleteButton, 2);
        internalGrid.Add(new Label
        {
            Text = $"Age: {_age.ToString()}",
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start
        }, 0);
        internalGrid.Add(new Label
        {
            Text = color.ToColorString(),
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End
        }, 0, 1);
    }

    private async void DeleteButtonOnClicked(object sender, EventArgs e)
    {
        await SettingsManager.TryRemoveColor(_age);
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.BuildColors();
        }
    }

    private async void OnTapped(object sender, EventArgs e) 
        => await Shell.Current.ShowPopupAsync(new Popups.ColorPopup());
}