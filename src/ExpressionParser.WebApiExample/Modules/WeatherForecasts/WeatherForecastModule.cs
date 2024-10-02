using ExpressionParser.WebApiExample.Common;

namespace ExpressionParser.WebApiExample.Modules.WeatherForecasts;

/// <summary>
/// Provides API endpoints for WeatherForecast as a module
/// </summary>
internal class WeatherForecastModule : IEndpointsModule
{
    /// <summary>
    /// Map all module endpoints
    /// </summary>
    /// <param name="app"></param>
    public void MapEndpoints(WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

}
