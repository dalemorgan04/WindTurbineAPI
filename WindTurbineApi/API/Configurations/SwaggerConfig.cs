using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WindTurbineApi.API.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Wind Turbine API",
                    Version = "v1",
                    Description = "An API for managing wind turbine sensors, and data records.",
                    Contact = new OpenApiContact
                    {
                        Name = "Dale Morgan",
                        Email = "dalemorgan04@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Get the XML comments file path
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Tell Swagger to include XML comments
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wind Turbine API v1");
                c.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}