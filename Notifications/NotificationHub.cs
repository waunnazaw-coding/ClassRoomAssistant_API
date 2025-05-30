using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ClassRoomClone_App.Server.Notifications;

[Authorize]
public class NotificationHub : Hub
{
    
}