using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace bull_chat_backend.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerExtensions(this IServiceCollection services)
        {
            var apiTitle = "Bull Chat API";
            var apiVersion = "v1";
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });

                // Добавляем JWT-авторизацию в Swagger
                c.AddJwtSecurityDefinition();
                c.AddJwtSecurityRequirement();
                // Добовляем фильтр для IFormFile
                //c.OperationFilter<FileUploadOperationFilter>();
            });

            return services;
        }

        // Отдельный метод расширения для добавления определения безопасности (JWT)
        private static void AddJwtSecurityDefinition(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });
        }

        // Отдельный метод расширения для добавления требования безопасности (JWT)
        private static void AddJwtSecurityRequirement(this SwaggerGenOptions options)
        {
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
