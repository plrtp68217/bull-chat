using bull_chat_backend.Models.DBase;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend
{
    public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
    {
        public DbSet<Content> Content { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Content);
        }
    }
}
