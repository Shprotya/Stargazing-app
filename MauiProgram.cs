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

            // Register services
            // Services registered as Singleton live for the entire application lifetime.
            builder.Services.AddSingleton<NasaApiService>(); 
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
