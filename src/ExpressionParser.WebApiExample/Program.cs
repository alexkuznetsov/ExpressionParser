
using System.Reflection;

using ExpressionParser.WebApiExample.Common;
using ExpressionParser.WebApiExample.Data;
using ExpressionParser.WebApiExample.Modules.Cities;

namespace ExpressionParser.WebApiExample;

public class Program
{
    private static readonly Assembly MyselfAssembly = typeof(Program).Assembly;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssembly(MyselfAssembly);
            c.RegisterGenericHandlers = false;
        });

        builder.Services.AddEndpointModulesFromAssembly(MyselfAssembly);
        builder.Services
            .AddData()
            .AddCitiesData();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseEndpointModules();

        app.Run();
    }
}
