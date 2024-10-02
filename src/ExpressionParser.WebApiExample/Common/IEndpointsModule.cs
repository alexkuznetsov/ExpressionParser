using System.Reflection;

namespace ExpressionParser.WebApiExample.Common;

/// <summary>
/// Provides modular functionality for endpoints 
/// </summary>
public interface IEndpointsModule
{
    /// <summary>
    /// Map all the module endpoints
    /// </summary>
    /// <param name="app"></param>
    void MapEndpoints(WebApplication app);
}


internal static class Extensions
{
    private static readonly Type ThisModuleServiceType = typeof(IEndpointsModule);

    internal static IServiceCollection AddEndpointModulesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && t.IsAssignableTo(ThisModuleServiceType));

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(ThisModuleServiceType, type, ServiceLifetime.Transient));
        }

        return services;
    }

    internal static WebApplication UseEndpointModules(this WebApplication app)
    {
        var services = app.Services.GetServices<IEndpointsModule>();

        foreach (var service in services)
        {
            service.MapEndpoints(app);
        }

        return app;
    }
}