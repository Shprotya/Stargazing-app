using StargazingApp.ViewModels;

namespace StargazingApp.Views;

// Follows the MVVM pattern by connecting the View to the ViewModel.
public partial class MainPage : ContentPage
{
    // Constructor that receives the MainViewModel through dependency injection.
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();

        // Set the BindingContext - this connects all {Binding} expressions in XAML 
        BindingContext = viewModel;
    }

    // This ensures fresh data is loaded each time the user navigates to this page.
    protected override void OnAppearing()
    {
        // Call the base implementation first
        base.OnAppearing();

        // Cast the BindingContext back to MainViewModel to access its commands
        var vm = (MainViewModel)BindingContext;

        // This will trigger the LoadApod() method in the ViewModel
        vm.LoadApodCommand.Execute(null);
        
    }
}