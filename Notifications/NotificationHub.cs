using System.Security.Claims;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Notifications
{
    [Authorize]
    public sealed class NotificationsHub : Hub<INotificationsClient>
    {
        public async Task SendNotification(NotificationPayload content)
        {
            await Clients.All.ReceiveNotification(content);
        }
    }
}
