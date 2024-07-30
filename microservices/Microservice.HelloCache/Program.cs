using Microservice.HelloWorld;
using Microsoft.AspNetCore.Mvc;
using Microservice.HelloCache;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

await WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .ConfigureMiddleware()
    .ConfigureHandlers()
    .RunAsync();

public static partial class Program
{
    public static WebApplicationBuilder ConfigureAppsettings(this WebApplicationBuilder builder)
    {
        builder.Configuration.Bind((Appsettings appsettings) => builder.Configuration.Bind(appsettings));
        return builder;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<PersonHandler>();

        return builder;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        var configuration = new Appsettings(app.Configuration);
        app.MigratePostgresql(options =>
        {
            options.ConnectionString = "";
            options.MigrationClientId = configuration.Postgres.MigrationIdentity;
            options.UseManagedIdentity = true;
        });

        return app;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        return app;
    }

    public static WebApplication ConfigureHandlers(this WebApplication app)
    {
        var persons = app.MapGroup("persons");

        persons.MapPost("/", async ([FromServices] PersonHandler handler, [FromBody] PersonInputModel person) => await handler.CreatePerson(person));
        persons.MapGet("/{id:guid}", async ([FromServices] PersonHandler handler, [FromRoute] Guid id) => await handler.GetPerson(id));

        return app;
    }
}