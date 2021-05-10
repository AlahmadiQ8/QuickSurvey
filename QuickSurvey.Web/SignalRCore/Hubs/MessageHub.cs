using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using QuickSurvey.Web.Extensions;

namespace QuickSurvey.Web.SignalRCore.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly ILogger<MessageHub> _logger;
        private readonly IUserConnectionRepository _userConnections;

        public MessageHub(ILogger<MessageHub> logger, IUserConnectionRepository userConnections)
        {
            _logger = logger;
            _userConnections = userConnections;
        }

        [HubMethodName("NewMessage")]
        public async Task NewMessage(string username, string message)
        {
            var sessionId = Context.User.GetSessionId();
            await Clients.Group(sessionId).SendAsync("messageReceived", $"group {sessionId}", message);
        }

        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.User.GetSessionId();

            if (await _userConnections.UserInGroup(sessionId, Context.UserIdentifier))
            {
                var prevConnectionId = await _userConnections.RemoveUserConnectionFromGroup(sessionId, Context.UserIdentifier);
                await Groups.RemoveFromGroupAsync(prevConnectionId, sessionId);
            }

            await _userConnections.AddUserConnectionToGroup(sessionId, Context.UserIdentifier, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            var activeUsers = await _userConnections.GetUsers(sessionId);

            await Clients.Group(sessionId).SendAsync(SignalRServerMethods.ActiveUsersUpdated, activeUsers);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var sessionId = Context.User.GetSessionId();

            if (exception != null)
            {
                _logger.LogError(exception.StackTrace);
            }
            await _userConnections.RemoveUserConnectionFromGroup(sessionId, Context.UserIdentifier);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);

            var activeUsers = await _userConnections.GetUsers(sessionId);

            await Clients.Group(sessionId).SendAsync(SignalRServerMethods.ActiveUsersUpdated, activeUsers);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
