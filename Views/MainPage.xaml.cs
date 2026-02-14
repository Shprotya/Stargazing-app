using StargazingApp.ViewModels;

namespace StargazingApp.Views;

/// <summary>
/// This class handles the initialization and lifecycle events of the page.
/// Follows the MVVM pattern by connecting the View to the ViewModel.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Constructor that receives the MainViewModel through dependency injection.
    /// Sets up the binding context to connect the XAML UI to the ViewModel properties.
    /// </summary>
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();

        // Set the BindingContext - this connects all {Binding} expressions in XAML
        BindingContext = viewModel;
    }

    /// <summary>
    /// Called when the page is about to appear on the screen.
    /// Automatically triggers loading of NASA's Picture of the Day data.
    /// This ensures fresh data is loaded each time the user navigates to this page.
    /// </summary>
    protected override void OnAppearing()
    {
        // Call the base implementation first
        base.OnAppearing();

        // Cast the BindingContext back to MainViewModel to access its commands
        var vm = (MainViewModel)BindingContext;

        // Execute the command to load APOD data
        // This will trigger the LoadApod() method in the ViewModel
        vm.LoadApodCommand.Execute(null);
        
    }
}