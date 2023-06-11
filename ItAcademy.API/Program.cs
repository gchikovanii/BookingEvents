using FluentValidation.AspNetCore;
using FluentValidation;
using ItAcademy.API.Infrastructure.Auth.Jwt;
using ItAcademy.API.Infrastructure.Extensions.Auth;
using ItAcademy.API.Infrastructure.Extensions.Culture;
using ItAcademy.API.Infrastructure.Middleware.Exceptions;
using ItAcademy.Persistence.Connections;
using ItAcademy.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using ItAcademy.API.Infrastructure.VersioningSwagger;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ItAcademy.Persistence.DatSeed;
using ItAcademy.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using ItAcademy.Infrastructure.Infrastructures.ServiceMiddleware;
using ItAcademy.Application.Infrastructure.Mapping;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

#region SeriLog 
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersionedApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});
builder.Services.AddApiVersioning();
#region Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.OperationFilter<SwaggerDefaultValues>();
    option.ExampleFilters();
    //Jwt Bearer
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization Bearer Scheme"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
}).AddSwaggerGenNewtonsoftSupport();
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

#endregion
builder.Services.AddServices();
builder.Services.AddMaps();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ItAcademyDbContext>();

#region Flunt Validations 
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
#endregion
#region DBConfiguration
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));
builder.Services.AddDbContext<ItAcademyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnectionString)));
});

#endregion
#region JwtConfiguration
builder.Services.AddTokenAuth(builder.Configuration.GetSection(nameof(JwtConfiguration)).GetSection(nameof(JwtConfiguration.Secret)).Value);
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)));
#endregion

#region HealtChecks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnectionString)));
#endregion
var app = builder.Build();

#region Seed Data
SeedData.Initialize(app.Services);
UserSeed.SeedUsersAndRolesAsync(app).Wait();
#endregion
#region Swagger with vers
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        foreach (var desciptions in provider.ApiVersionDescriptions)
        {
            option.SwaggerEndpoint($"/swagger/{desciptions.GroupName}/swagger.json"
                , $"{desciptions.GroupName.ToUpper()}");
        }
    });
}
#endregion

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseCulture();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    #region Healt Checks
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecks("/quickHealth", new HealthCheckOptions()
    {
        Predicate = _ => false
    });
    #endregion
    endpoints.MapControllers();
});

#region AppRun
try
{
    app.Run();
}
catch (Exception ex)
{

#pragma warning disable CA1305 // Specify IFormatProvider
    using (var res = new LoggerConfiguration().WriteTo.File(@"Logs\fatalExceptions.txt").CreateLogger())
    {
        res.Fatal(ex, "Host Crashed!");
    }
#pragma warning restore CA1305 // Specify IFormatProvider

}
#endregion
