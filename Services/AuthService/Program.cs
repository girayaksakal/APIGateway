// Authentication and Authorization
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Versioning
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
// Swagger
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
// Database and migrations
using SharedService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Authentication and Authorization
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "LcWVcTb9JdFaZ43oRDk1GZR19gApAyi3cIH8wDWsF/M=")) // REMOVE DEFAULT SECRET KEY IN PRODUCTION
        };
    });
builder.Services.AddAuthorization();

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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
// Database connection
builder.Services.AddDbContext<SharedDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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
            options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
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