using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly IClassParticipantsRepository _classParticipantRepo;
    private readonly DbContextClassName _context;

    public MessageService(
        IMessageRepository messageRepo,
        INotificationRepository notificationRepo,
        IAnnouncementRepository announcementRepo,
        IClassParticipantsRepository classParticipantRepo,
        DbContextClassName context)
    {
        _messageRepo = messageRepo;
        _notificationRepo = notificationRepo;
        _announcementRepo = announcementRepo;
        _classParticipantRepo = classParticipantRepo;
        _context = context;
    }

    public async Task<MessageResponseDto> CreateMessageAsync(MessageCreateRequestDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var message = new Message
        {
            SenderId = dto.SenderId,
            ReceiverId = dto.IsPrivate ? dto.ReceiverId : null,
            ParentType = dto.ParentType,
            ParentId = dto.ParentId,
            Message1 = dto.Content,
            IsPrivate = dto.IsPrivate,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepo.AddAsync(message);

        IEnumerable<int> notifyUserIds;

        if (dto.IsPrivate)
        {
            if (!dto.ReceiverId.HasValue)
                throw new ArgumentException("ReceiverId must be set for private messages");

            notifyUserIds = new[] { dto.ReceiverId.Value };
        }
        else
        {
            // For public messages, notify all class participants of the related announcement
            int classId = 0;
            if (dto.ParentType == "Announcement")
            {
                var announcement = await _announcementRepo.GetByIdAsync(dto.ParentId);
                if (announcement == null)
                    throw new KeyNotFoundException("Announcement not found");
                classId = announcement.ClassId;
            }
            else
            {
                // Handle other ParentTypes if needed
                throw new NotSupportedException("Only Announcement parent type supported currently");
            }

            notifyUserIds = await _classParticipantRepo.GetUserIdsByClassIdAsync(classId);
        }

        var notifications = notifyUserIds.Select(userId => new Notification
        {
            UserId = userId,
            Type = "Message",
            ReferenceId = message.Id,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        await _notificationRepo.AddRangeAsync(notifications);

        await transaction.CommitAsync();

        return new MessageResponseDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            ParentType = message.ParentType,
            ParentId = message.ParentId,
            Content = message.Message1,
            IsPrivate = message.IsPrivate,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task UpdateMessageAsync(int id, MessageCreateRequestDto dto)
    {
        var message = await _messageRepo.GetByIdAsync(id);
        if (message == null)
            throw new KeyNotFoundException("Message not found");

        message.Message1 = dto.Content;
        message.IsPrivate = dto.IsPrivate;

        await _messageRepo.UpdateAsync(message);
    }

    public async Task DeleteMessageAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        await _notificationRepo.DeleteByReferenceAsync("Message", id);
        await _messageRepo.DeleteAsync(id);

        await transaction.CommitAsync();
    }
}