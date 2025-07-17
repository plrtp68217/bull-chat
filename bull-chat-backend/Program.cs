using bull_chat_backend.Hubs;
using bull_chat_backend.Mapper;
using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using bull_chat_backend.Extensions;
using Microsoft.OpenApi.Models;
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
            builder.Services.AddAutoMapper(typeof(MappingProfile));

      

            builder.Services.AddControllers();

            builder.Services.AddTransient<IUserRegistrationService, UserRegistrationService>();
            builder.Services.AddTransient<IJwtGenerator<User>, JwtGeneratorService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
            builder.Services.AddTransient<IMessageRepository, MessageRepository>();
            builder.Services.AddTransient<IContentRepository, ContentRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            /* 
                INFO: Порядок важен!!!
                Здесь по сути строится pipeline middleware

                Так же есть разница между Use и Run!

                app.Use(async (context, next) =>
                {
                    // Код до вызова следующего middleware
                    Console.WriteLine("Before next middleware");

                    await next(); // Вызов следующего middleware

                    // Код после выполнения следующего middleware
                    Console.WriteLine("After next middleware");
                });


                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello, World!");
                    // После этого запрос завершается, дальше ничего не выполняется
                });
            */
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
