
using bull_chat_backend.Hubs;
using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

namespace bull_chat_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

            builder.Services.AddPgsqlConnection(connectionString!);

            builder.Services.AddAuthorization();

            //swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

            builder.Services.AddControllers();

            builder.Services.AddTransient<UserRegistrationService>();

            builder.Services.AddTransient<IJwtGenerator<User>, JwtGeneratorService>();
            
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
            builder.Services.AddTransient<IMessageRepository, MessageRepository>();
            builder.Services.AddTransient<IContentRepository, ContentRepository>();
 

            var app = builder.Build();

            app.UseAuthentication(); // Важно! До UseAuthorization
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();

            app.UseRouting();

            app.MapHub<ChatHub>("/chat");

            app.Map("/test", async (IUserRepository userRepository, CancellationToken token) =>
            {
                return await userRepository.CountAsync(token);
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.Run();
        }
    }
}
