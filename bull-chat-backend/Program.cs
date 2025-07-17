
using AutoMapper;
using bull_chat_backend.Hubs;
using bull_chat_backend.Mapper;
using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

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
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(nameof(JwtOptions)));

            builder.Services.AddControllers();

            builder.Services.AddTransient<IUserRegistrationService, UserRegistrationService>();
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

            app.MapHub<ChatHub>(ChatHub.HUB_URI);

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
