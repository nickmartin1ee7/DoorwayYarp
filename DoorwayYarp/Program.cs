var builder = WebApplication.CreateBuilder(args);

builder.Configuration
#if DEBUG
    .AddJsonFile("data/proxysettings.json", optional: true)
#else
    .AddJsonFile("data/proxysettings.json", optional: false)
#endif
    .Build();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();

