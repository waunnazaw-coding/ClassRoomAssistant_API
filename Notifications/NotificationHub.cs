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
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IClassParticipantsRepository _classParticipantRepository;
        private readonly DbContextClassName _dbContext; // Assume EF Core DbContext injected

        public NotificationHub(
            ILogger<NotificationHub> logger,
            IClassParticipantsRepository classParticipantRepository,
            DbContextClassName dbContext)
        {
            _logger = logger;
            _classParticipantRepository = classParticipantRepository;
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                _logger.LogWarning("Connection {ConnectionId} has no valid user ID.", Context.ConnectionId);
                return;
            }

            // Persist connection in DB
            var userConnection = new UserConnection
            {
                UserId = userId.Value,
                ConnectionId = Context.ConnectionId,
                ConnectedAt = DateTime.UtcNow
            };

            _dbContext.UserConnections.Add(userConnection);
            await _dbContext.SaveChangesAsync();

            // Add connection to user-specific group for private notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));

            _logger.LogInformation("User connected: UserId={UserId}, ConnectionId={ConnectionId}",
                userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                // Remove connection from DB
                var connection = await _dbContext.UserConnections
                    .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId && c.UserId == userId.Value);

                if (connection != null)
                {
                    _dbContext.UserConnections.Remove(connection);
                    await _dbContext.SaveChangesAsync();
                }

                // Remove from user group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));

                _logger.LogInformation("User disconnected: UserId={UserId}, ConnectionId={ConnectionId}",
                    userId, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("Disconnected connection {ConnectionId} had no valid user ID.", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join class group only if user is participant in class.
        /// </summary>
        public async Task JoinClassGroup(int classId)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new HubException("User not authenticated.");

            var role = await _classParticipantRepository.GetRetrieveRoleAsyn(userId.Value, classId);
            if (role == null)
                throw new HubException("You are not a participant of this class.");

            await Groups.AddToGroupAsync(Context.ConnectionId, GetClassGroupName(classId));
            _logger.LogInformation("User {UserId} joined group Class_{ClassId}", userId, classId);
        }

        /// <summary>
        /// Leave class group only if user is participant in class.
        /// </summary>
        public async Task LeaveClassGroup(int classId)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new HubException("User not authenticated.");

            var role = await _classParticipantRepository.GetRetrieveRoleAsyn(userId.Value, classId);
            if (role == null)
                throw new HubException("You are not a participant of this class.");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetClassGroupName(classId));
            _logger.LogInformation("User {UserId} left group Class_{ClassId}", userId, classId);
        }

        /// <summary>
        /// Only Teachers or SubTeachers can send announcements.
        // </summary>
        public async Task SendClassAnnouncement(int classId, string message)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new HubException("User not authenticated.");

            var role = await _classParticipantRepository.GetRetrieveRoleAsyn(userId.Value, classId);
            if (role == null || (role != "Teacher" && role != "SubTeacher"))
                throw new HubException("Only teachers or subteachers can send announcements.");

            await Clients.Group(GetClassGroupName(classId)).SendAsync("ReceiveAnnouncement", message);
            _logger.LogInformation("User {UserId} sent announcement to Class_{ClassId}", userId, classId);
        }

        /// <summary>
        /// Only Students can send questions.
        /// </summary>
        public async Task SendStudentQuestion(int classId, string question)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new HubException("User not authenticated.");

            var role = await _classParticipantRepository.GetRetrieveRoleAsyn(userId.Value, classId);
            if (role == null || role != "Student")
                throw new HubException("Only students can send questions.");

            await Clients.Group(GetClassGroupName(classId)).SendAsync("ReceiveStudentQuestion", question);
            _logger.LogInformation("User {UserId} sent question to Class_{ClassId}", userId, classId);
        }

        /// <summary>
        /// Send notification to all connections of a specific user.
        /// </summary>
        public async Task SendNotificationToUser(int userId, string message)
        {
            // Send to all connections in the user group
            await Clients.Group(GetUserGroupName(userId)).SendAsync("ReceiveNotification", message);
            _logger.LogInformation("Sent notification to UserId={UserId}: {Message}", userId, message);
        }

        #region Helpers

        private int? GetUserId()
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out var userId) ? userId : (int?)null;
        }

        public static string GetUserGroupName(int userId) => $"User_{userId}";
        public static string GetClassGroupName(int classId) => $"Class_{classId}";

        #endregion
    }
}
