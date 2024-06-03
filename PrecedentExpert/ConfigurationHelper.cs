using Microsoft.Extensions.Configuration;

public static class ConfigurationHelper
{
   public static IConfigurationRoot GetConfiguration()
    {
        var basePath = AppContext.BaseDirectory;
        Console.WriteLine($"Base directory: {basePath}");
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    }

    public static string GetConnectionString(string name)
    {
        var configuration = GetConfiguration();
        return configuration.GetConnectionString(name);
    }
    
}
