namespace EffettoMandria.Api;

/// <summary>
/// Applica Hosting:Environment e Hosting:Urls da appsettings (soprattutto Production)
/// quando le variabili d'ambiente non sono già impostate (es. deploy in publish/).
/// </summary>
public static class HostingBootstrap
{
    public static void ApplyFromAppSettings(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var environment = config["Hosting:Environment"];
        if (!string.IsNullOrWhiteSpace(environment) &&
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        }

        var urls = config["Hosting:Urls"];
        if (!string.IsNullOrWhiteSpace(urls) &&
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", urls);
        }
    }
}
