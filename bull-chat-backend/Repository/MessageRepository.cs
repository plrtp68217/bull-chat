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
            _context.User.Attach(user);
            var msg = new Message(user, item);
            await _context.Message.AddAsync(msg, token);
            await _context.SaveChangesAsync(token);
            return msg;
        }

        public async ValueTask<DateTime> LastMessageDate(CancellationToken token)
        {
            var lastMessage = await _context.Message
                .OrderByDescending(m => m.Date)
                .FirstAsync(token);

            return lastMessage.Date;
        }

        public async Task<ICollection<MessageDto>> GetPagedMessages(
            DateTime? cursor,
            bool isNext,
            CancellationToken token,
            int pageSize)
        {
            var query = _context.Message
                .AsNoTracking()
                .Include(m => m.User)
                .Include(m => m.Content)
                .AsQueryable();

            if (cursor.HasValue)
            {
                query = isNext
                    ? query.Where(m => m.Date < cursor.Value)  
                    : query.Where(m => m.Date > cursor.Value);
            }

            query = isNext
                ? query.OrderByDescending(m => m.Date)  
                : query.OrderBy(m => m.Date);           

            var page = await query
                .Take(pageSize)
                .ToListAsync(token);

            var result = isNext
                ? page.OrderBy(m => m.Date)             
                : page.OrderByDescending(m => m.Date); 

            return [.. result.Select(m => m.ToDto())];
        }

    }
}
