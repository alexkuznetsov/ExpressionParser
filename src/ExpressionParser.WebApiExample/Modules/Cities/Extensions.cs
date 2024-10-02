namespace ExpressionParser.WebApiExample.Modules.Cities;

internal static class Extensions
{
    internal static IServiceCollection AddCitiesData(this IServiceCollection services)
    {
        services.AddSingleton<QueryMapping<City>, CityMapping>();
        services.AddTransient<ICitiesRepository, CitiesRepositoryImpl>();

        return services;
    }
}
