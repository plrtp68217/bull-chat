using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Extensions
{
    public static class PsqlExtension
    {
        public static IServiceCollection AddPgsqlConnection(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChatDbContext>(options =>
             {
                 options.UseNpgsql(connectionString, npgsqlOptions =>
                 {
                     npgsqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 5,
                         maxRetryDelay: TimeSpan.FromSeconds(30),
                         errorCodesToAdd: null);

                     npgsqlOptions.CommandTimeout(30);
                 });
             });
            return services;
        }
    }
}
