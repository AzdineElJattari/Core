using Microsoft.Extensions.Configuration;
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

Log.Logger.Information("Application starting!");

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {

    }).UseSerilog().Build();

static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables(); // This is the setup to talk to your configuration source
}

public class GreetingService
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