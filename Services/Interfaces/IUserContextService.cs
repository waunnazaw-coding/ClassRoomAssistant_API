namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IUserContextService
{
    int GetCurrentUserId();
    string GetCurrentUserEmail();
    bool IsAuthenticated();
}
