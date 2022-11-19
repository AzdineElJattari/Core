using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = new ConfigurationBuilder();
BuildConfig(builder);

//Setup for serilog with builder
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Application is starting!");

// Setup DI
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Means give me a new instance everytime asked for the GreetingService -->  services.AddTransient<IGreetingService, GreetingService>();
        services.AddTransient<GreetingService>();

    }).UseSerilog().Build();
var svc = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
svc.Run(); // svc variable gaat via CreateInstance() methode vragen aan de host om te kijken in zijn services en daar een instance van het meegegeven type terug te geven in deze geval IGreetingService


// Setup configuration
static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables(); // This is the setup to talk to your configuration source
}

// Setup Interface & Class for logging something 
public interface IGreetingService
{
    void Run();
}

public class GreetingService : IGreetingService
{
    private readonly ILogger<GreetingService> logger;
    private readonly IConfiguration config;

    public GreetingService(ILogger<GreetingService> logger, IConfiguration config)
    {
        this.logger = logger;
        this.config = config;
    }

    public void Run()
    {
        for (int i = 0; i < config.GetValue<int>("LoopTimes"); i++)
        {
            logger.LogInformation("Run number {runNumber}", i);
        }
    }
}