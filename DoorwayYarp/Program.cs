var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("data/proxysettings.json", optional: false)
    .Build();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();

