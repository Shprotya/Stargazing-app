using StargazingApp.Services;

namespace StargazingApp;

public partial class App : Application
{
    public App(DatabaseService databaseService)
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}