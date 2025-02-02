using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context
            .Messages.OrderByDescending(m => m.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }


    public async Task<IEnumerable<MessageDto>?> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender).ThenInclude(s => s.Photos)
            .Include(m => m.Recipient).ThenInclude(r => r.Photos)
            .Where(
                m => m.SenderUsername == currentUsername && m.SenderDeleted == false && m.RecipientUsername == recipientUsername
                || m.SenderUsername == recipientUsername && m.RecipientDeleted == false && m.RecipientUsername == currentUsername
            )
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername);

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}