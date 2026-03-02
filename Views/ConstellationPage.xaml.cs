namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    private readonly ConstellationViewModel _viewModel;

    public ConstellationPage(ConstellationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Initial load
        await _viewModel.LoadAllCommand.ExecuteAsync(null);
    }
}