using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DBase.Enum;
using bull_chat_backend.Models.DTO;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext _context;

        public MessageRepository(ChatDbContext context) => _context = context;

        public async Task AddAsync(Message entity, CancellationToken token)
        {
            await _context.Message.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);
        }

        public async Task AddRangeAsync(IEnumerable<Message> entities, CancellationToken token)
        {
            await _context.Message.AddRangeAsync(entities);
            await _context.SaveChangesAsync(token);
        }

        public async Task<Message> GetByIdAsync(int id, CancellationToken token)
        {
            return await _context.Message.FindAsync(id, token) ?? Message.Empty;
        }

        public async Task<IEnumerable<Message>> GetAllAsync(CancellationToken token)
        {
            return await _context.Message.ToListAsync(token);
        }

        public async Task UpdateAsync(Message entity, CancellationToken token)
        {
            _context.Message.Update(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(Message entity, CancellationToken token)
        {
            _context.Message.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken token)
        {
            var message = await _context.Message.FindAsync(id, token);

            if (message != null)
            {
                _context.Message.Remove(message);
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken token)
        {
            return await _context.Message.AnyAsync(m => m.Id == id, token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _context.Message.CountAsync(token);
        }

        public async Task<Message> AddAsync(User user, string item, ContentType contentType, CancellationToken token)
        {
            var msg = new Message()
            {
                User = user,
                Content = new()
                {
                    ContentType = contentType,
                    Item = item
                }
            };
            await _context.Message.AddAsync(msg, token);
            await _context.SaveChangesAsync(token);
            return msg;
        }


        public async Task<ICollection<MessageDto>> GetLastNFromDateAsync(int count, DateTime fromDate)
        {
            return await _context.Message
                .Include(m => m.User)
                .Include(m => m.Content)
                .Where(m => m.Date >= fromDate)
                .OrderByDescending(m => m.Date)
                .Take(count)
                .OrderBy(m => m.Date)
                .Select(m => m.ToDto())
                .ToListAsync();
        }

        public async Task<ICollection<MessageDto>> GetDateIntervalAsync(DateTime dateStart, DateTime dateTo)
        {
            return await _context.Message
                .Where(m => m.Date >= dateStart && m.Date <= dateTo)
                .Include(m => m.User)
                .Include(m => m.Content)
                .OrderBy(m => m.Date)
                .Select(m => m.ToDto())
                .ToListAsync();

        }
    }
}
