using StargazingApp.ViewModels;

namespace StargazingApp.Views;

public partial class MainPage : ContentPage
{
    // We inject the ViewMode
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // This connects the XAML to the ViewModel
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Tell the ViewModel to load the data automatically
        var vm = (MainViewModel)BindingContext;
        if (vm.LoadApodCommand.CanExecute(null))
        {
            vm.LoadApodCommand.Execute(null);
        }
    }
}