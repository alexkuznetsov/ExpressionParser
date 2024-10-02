using ExpressionParser.WebApiExample.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ExpressionParser.WebApiExample.Modules.Cities;

internal sealed class CitiesModule : IEndpointsModule
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("api/cities", GetHandler)
            .WithName("GetCitiesHandler")
            .WithOpenApi();
    }

    private static async Task<IResult> GetHandler(ISender sender, Query query)
    {
        var result = await sender.Send(new GetCitiesQuery(new(query.Name, query.Province, query.Limit, query.Offset)));
        return Results.Ok(result.Cities);
    }

    public record Query
    {
        public string? Name { get; set; }

        public string? Province { get; set; }

        public int? Limit { get; set; } = 100;

        public int? Offset { get; set; } = 0;

        public static ValueTask<Query> BindAsync(HttpContext context)
        {
            var quety = new Query();
            quety.Offset = context.Request.Query.ContainsKey("offset") && int.TryParse(context.Request.Query["offset"], out var t1) ? t1 : 0;
            quety.Limit = context.Request.Query.ContainsKey("limit") && int.TryParse(context.Request.Query["limit"], out var t2) ? t2 : 0;
            quety.Name = context.Request.Query.ContainsKey("name") ? context.Request.Query["name"].ToString() : null!;
            quety.Province = context.Request.Query.ContainsKey("province") ? context.Request.Query["province"].ToString() : null!;
            return ValueTask.FromResult(quety);
        }
    }
}
