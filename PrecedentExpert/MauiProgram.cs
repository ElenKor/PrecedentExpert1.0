
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using PrecedentExpert.ViewModels;
using PrecedentExpert.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace PrecedentExpert
{
    public static class MauiProgram
    {
        private static IServiceProvider _serviceProvider;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Add configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(configuration);

            builder.UseMauiApp<App>().UseMauiCommunityToolkit().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            // Настройка логирования
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString)
                       .LogTo(Console.WriteLine, LogLevel.Debug)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors();
            });

            // Регистрация ViewModel
            builder.Services.AddScoped<ObjectsViewModel>();
            builder.Services.AddScoped<SituationVariablesViewModel>();
            builder.Services.AddScoped<SolutionVariablesViewModel>();
            builder.Services.AddScoped<PrecedentSearchViewModel>();
            builder.Services.AddTransient<SituationVariantsForObjectViewModel>();
            builder.Services.AddScoped<SolutionVariantsForObjectViewModel>();
            builder.Services.AddScoped<SelectObjectViewModel>();
            builder.Services.AddScoped<ObjectViewModel>();

            _serviceProvider = builder.Services.BuildServiceProvider();

            // Ensure the database is created and apply migrations if needed
            using (var scope = _serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();

                    if (!context.Database.CanConnect() || !context.Database.GetService<IRelationalDatabaseCreator>().Exists())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during migration: {ex.Message}");
                    throw;
                }
            }

            return builder.Build();
        }

        public static IServiceProvider Services => _serviceProvider;
    }
}
