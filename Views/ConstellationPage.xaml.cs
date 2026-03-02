namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    private readonly ConstellationViewModel _viewModel;

    public ConstellationPage(ConstellationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to filter changes to update button colors
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Initial load
        await _viewModel.LoadAllCommand.ExecuteAsync(null);
        UpdateButtonColors(); // Set initial button state
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Update button colors when active filter changes
        if (e.PropertyName == nameof(ConstellationViewModel.ActiveFilter))
        {
            UpdateButtonColors();
        }
    }

    private void UpdateButtonColors()
    {
        var activeColor = (Color)Application.Current.Resources["NovaPurple"];
        var inactiveColor = (Color)Application.Current.Resources["Button"];

        BtnAll.BackgroundColor = _viewModel.ActiveFilter == "All" ? activeColor : inactiveColor;
        BtnNorth.BackgroundColor = _viewModel.ActiveFilter == "Northern" ? activeColor : inactiveColor;
        BtnSouth.BackgroundColor = _viewModel.ActiveFilter == "Southern" ? activeColor : inactiveColor;
        BtnFav.BackgroundColor = _viewModel.ActiveFilter == "Favorites" ? activeColor : inactiveColor;
    }
}