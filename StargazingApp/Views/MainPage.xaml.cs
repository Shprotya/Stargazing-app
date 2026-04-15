using StargazingApp.ViewModels;

namespace StargazingApp.Views;

// Follows the MVVM pattern by connecting the View to the ViewModel.
public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel;

    // Constructor that receives the MainViewModel through dependency injection.
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        BindingContext = this.viewModel;

        // Set the month label text directly — avoids XAML sys: namespace complexity
        TonightMonthLabel.Text = $"Best viewing for {DateTime.Now:MMMM}";
    }

    // This ensures fresh data is loaded each time the user navigates to this page.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadApodCommand.ExecuteAsync(null);
    }
}