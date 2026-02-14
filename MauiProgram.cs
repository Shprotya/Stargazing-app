using Microsoft.Extensions.Logging;
using StargazingApp.ViewModels;
using StargazingApp.Views;    

namespace StargazingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Services registered as Singleton live for the entire application lifetime.

            // Register services
            builder.Services.AddSingleton<NasaApiService>();

            // Register the View Models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<ConstellationViewModel>();

            // Register the Views
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ConstellationPage>();

            return builder.Build();
        }
    }
}
