
using bull_chat_backend.Repository;
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

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();

            builder.Services.AddTransient<IUserRepository, UserRepository>();

            var app = builder.Build();
            app.Map("/bull",async (IUserRepository userRepository, CancellationToken token) => {
                return await userRepository.CountAsync(token);
            });

            // Configure the HTTP request pipeline.
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
