using CommunityToolkit.Maui;
using Gauniv.Client.Services;
using Gauniv.Client.ViewModel;
using Microsoft.Extensions.Logging;

namespace Gauniv.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddSingleton<GameService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                return new GameService(httpClient);
            });
            builder.Services.AddTransient<IndexViewModel>();
            builder.Services.AddSingleton<HttpClient>(); // IndexViewModel doit être ajouté en tant que service
            //builder.Services.AddSingleton<LoginViewModel>();

#endif

            var app = builder.Build();

            Task.Run(() =>
            {
                // Initialisation du serveur ou autre si nécessaire
            });

            return app;
        }
    }
}
