using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDto> CreateMessageAsync(MessageCreateRequestDto dto);
    Task UpdateMessageAsync(int id, MessageCreateRequestDto dto);
    Task DeleteMessageAsync(int id);
}