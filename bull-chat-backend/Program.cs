using bull_chat_backend.Hubs;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using bull_chat_backend.Extensions;
namespace bull_chat_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //psql
            var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
            builder.Services.AddPgsqlConnection(connectionString!);

            builder.Services.AddAuthorization();

            //swagger
            builder.Services.AddEndpointsApiExplorer();

            builder.Services
                .ConfigureJwtOptions(builder.Configuration)
                .AddJwtAuthentication(builder.Configuration);

            builder.Services.AddSwaggerWithJwtAuth("Bull Chat API", "v1");

            builder.Services.AddSignalR();

            builder.Services.AddControllers();

            builder.Services.AddTransient<IUserRegistrationService, UserRegistrationService>();
            builder.Services.AddTransient<IJwtGenerator<User>, JwtGeneratorService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
            builder.Services.AddTransient<IMessageRepository, MessageRepository>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyFrontendPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyFrontendPolicy");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<ChatHub>(ChatHub.HUB_URI);

            app.Run();
        }
    }
}
