using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TodoLibrary.DataAccess;

namespace TodoAPI.StartupConfig;

public static class DependencyInjectionExtensions
{
    public static void AddStandardServices(this WebApplicationBuilder builder) {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwaggerServices();
    }

    private static void AddSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(opts =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            opts.AddSecurityDefinition("bearerAuth",
                new OpenApiSecurityScheme() {
                Name="Authorization",
                Type=SecuritySchemeType.Http,
                Description="Bearer token",
                Scheme="bearer",
                BearerFormat="JWT",
                In=ParameterLocation.Header
                });
            opts.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="bearerAuth"
                        }
                    }, new List<string>()
                },
            });
        });
    }

    public static void AddHealthCheckServices(this WebApplicationBuilder builder) {
        builder.Services.AddHealthChecks().AddSqlServer(
                builder.Configuration.GetConnectionString("Default")!);
    }

    public static void AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                                       builder.Configuration.GetValue<string>("Authentication:GEHEIM")!))
                };
            });
    }

    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        builder.Services.AddSingleton<ITodoData, TodoData>();
    }


}
