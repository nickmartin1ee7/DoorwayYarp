var builder = WebApplication.CreateBuilder(args);

TryAddReverseProxySettings(builder);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();

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