using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IAttachmentRepository
{
    Task AddRangeAsync(IEnumerable<Attachment> attachments);
    Task<List<Attachment>> GetByReferenceAsync(int referenceId, string referenceType);
    Task UpdateAsync(Attachment attachment);
    Task AddAsync(Attachment attachment);
    Task DeleteRangeAsync(IEnumerable<Attachment> attachments);
}