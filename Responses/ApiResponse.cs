using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Helpers;

public class ApiResponse<T>
{
    private IEnumerable<AnnouncementWithMessagesDto> announcements;
    private bool v1;
    private string v2;

    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public ApiResponse(T data, bool success = true, string message = null)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public ApiResponse(IEnumerable<AnnouncementWithMessagesDto> announcements, bool success = true, string message = null)
    {
        Success = success;
        Message = message;
        announcements = announcements;
          
    }
}
