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
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).HasMaxLength(300);

                entity.HasMany(u => u.Messages)
                    .WithOne(m => m.User)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Date).IsRequired();
                entity.Property(m => m.UserId).IsRequired();

                entity.HasOne(m => m.User)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Content)
                    .WithOne(c => c.Message)
                    .HasForeignKey<Content>(c => c.MessageId) // Content.MessageId - внешний ключ
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.MessageId)
                    .IsRequired();

                entity.Property(c => c.ContentType)
                    .HasConversion(
                        v => v == Models.DBase.Enum.ContentType.Text ? "txt" :
                             v == Models.DBase.Enum.ContentType.Image ? "img" :
                             v == Models.DBase.Enum.ContentType.Unknown ? "unk" : "unk",
                        v => v == "txt" ? Models.DBase.Enum.ContentType.Text :
                             v == "img" ? Models.DBase.Enum.ContentType.Image :
                             v == "unk" ? Models.DBase.Enum.ContentType.Unknown :
                             Models.DBase.Enum.ContentType.Unknown
                    )
                    .HasMaxLength(8);

                entity.Property(c => c.Item)
                    .IsRequired(true)
                    .HasMaxLength(2048);

                entity.HasOne(c => c.Message)
                    .WithOne(m => m.Content)
                    .HasForeignKey<Content>(c => c.MessageId) 
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
