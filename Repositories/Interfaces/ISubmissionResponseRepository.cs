using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface ISubmissionResponseRepository
{
    Task<SubmissionResponse?> GetByIdWithSubmissionAsync(int responseId);
    Task UpdateAsync(SubmissionResponse response);
    Task AddRangeAsync(IEnumerable<SubmissionResponse> responses);
    Task<SubmissionResponse> AddAsync(SubmissionResponse response);
}