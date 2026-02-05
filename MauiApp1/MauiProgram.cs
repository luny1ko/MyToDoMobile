using MauiApp1.Services;
using MauiApp1.Views;
using Microsoft.Extensions.Logging;

namespace MauiApp1;

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

        // Регистрация сервиса базы данных
        builder.Services.AddSingleton<DatabaseService>();

        // РЕГИСТРАЦИЯ СТРАНИЦ 
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<TaskDetailPage>();

        return builder.Build();
    }
}