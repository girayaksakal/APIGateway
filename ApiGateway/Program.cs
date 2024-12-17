// Ocelot
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
// Swagger
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
// Versioning
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add Ocelot and Configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShortStay API",
        Version = "v1",
        Description = "API for managing listings and bookings."
    });
});
//builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("http://localhost:5001/swagger/v1/swagger.json", "Auth Service");
    options.SwaggerEndpoint("http://localhost:5002/swagger/v1/swagger.json", "Booking Service");
    options.SwaggerEndpoint("http://localhost:5003/swagger/v1/swagger.json", "Listing Service");
});

// Configure middleware
app.UseRouting();
app.UseOcelot().Wait();

app.Run();

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Add a Swagger document for each discovered API version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"ShortStay API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "This API version has been deprecated."
                    : "API for managing listings and bookings."
            });
        }
    }
}