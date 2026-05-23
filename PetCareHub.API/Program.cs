using System.Reflection;
using Microsoft.OpenApi.Models;
using PetCareHub.API.Exceptions;
using PetCareHub.API.Extensions;

namespace PetCareHub.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddPetCareHubDbContext(builder.Configuration);
        
        builder.Services.AddPetCareHubRepositories();
        
        builder.Services.AddPetCareHubApplicationServices();
        
        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "🐾 PetCare Hub API",
                Version = "v1",
                Description = "API REST para gerenciamento de saúde de pets, consultas, alertas e indicadores clínicos",
                TermsOfService = new Uri("https://example.com/petcarehub/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Equipe PetCare Hub",
                    Email = "contato@petcarehub.com",
                    Url = new Uri("https://example.com/petcarehub")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://example.com/petcarehub/license")
                }
            });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        var app = builder.Build();
        
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare Hub API v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}