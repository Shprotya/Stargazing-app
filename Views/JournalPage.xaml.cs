using StargazingApp.ViewModels;

namespace StargazingApp.Views;

public partial class JournalPage : ContentPage
{
    private readonly JournalViewModel _viewModel;

    public JournalPage(JournalViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to ActiveFilter changes to keep button colours in sync
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAllCommand.ExecuteAsync(null);
        UpdateButtonColors();
    }

    // Event handling
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Client-side instant search using ISearchable
        // for already-loaded data, but we delegate to the ViewModel command for
        // consistency and so it also hits the DB for fresh data.
        _ = _viewModel.SearchCommand.ExecuteAsync(e.NewTextValue);
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(JournalViewModel.ActiveFilter))
            UpdateButtonColors();
    }

    // Helper
    private void UpdateButtonColors()
    {
        var active = (Color)Application.Current!.Resources["NovaPurple"];
        var inactive = (Color)Application.Current!.Resources["Button"];

        BtnAll.BackgroundColor = _viewModel.ActiveFilter == "All" ? active : inactive;
        BtnMonth.BackgroundColor = _viewModel.ActiveFilter == "ThisMonth" ? active : inactive;
    }
}
