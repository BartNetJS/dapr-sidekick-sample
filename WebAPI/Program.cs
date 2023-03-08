using Dapr.Client;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprSidekick(o =>
{
    o.Sidecar = new Man.Dapr.Sidekick.DaprSidecarOptions() {
            DaprHttpPort = DaprHttpPort
    };
    
});
builder.Services.AddDaprClient(o=>
{
    o.UseHttpEndpoint($"http://localhost:{DaprHttpPort}");
});

builder.Services.AddControllers().AddDapr();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/health/dapr", async (DaprClient daprClient) =>
{
    var health = await daprClient.CheckHealthAsync();
    if (health)
    {
        return Results.Ok(health);
    } else
    {
        return Results.Problem("Dapr sidecare unhealthy");
    }
})
.WithName("Dapr Health")
.WithOpenApi();


app.Run($"http://localhost:{ApplicationPort}");

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program
{
    public static int DaprHttpPort = 3501;
    public static int ApplicationPort = 8500;
}