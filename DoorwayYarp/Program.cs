using Serilog;

WebApplicationBuilder builder = null;

try
{
    Log.Information("Application Starting...");

    AppDomain.CurrentDomain.UnhandledException += (o, e) =>
    {
        Log.Logger.Fatal(e.ExceptionObject as Exception, "Unhandled Exception! Crashing...");
    };

    builder = WebApplication.CreateBuilder(args);

    TryAddReverseProxySettings(builder);
    ConfigureLogging(builder);

    builder.Services
        .AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    app.MapReverseProxy();

    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application Failed to start!");
    throw;
}

void ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Host.UseSerilog(Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .WriteTo.Seq(
            serverUrl: builder.Configuration
                .GetSection("Telemetry")
                .GetValue<string>("Url"),
            apiKey: builder.Configuration
                .GetSection("Telemetry")
                .GetValue<string>("Key"))
        .CreateLogger());
}

static void TryAddReverseProxySettings(WebApplicationBuilder builder)
{
    var reverseProxySettingsFilePath = builder.Configuration
        .GetSection("ReverseProxySettingsFilePath")
        .Get<string>();

    if (reverseProxySettingsFilePath != default)
    {
        builder.Configuration
            .AddJsonFile(reverseProxySettingsFilePath, optional: false);
    }
}