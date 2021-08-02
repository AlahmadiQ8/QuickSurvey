using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Web.Extensions;

namespace QuickSurvey.Web.SignalRCore.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly ILogger<MessageHub> _logger;
        private readonly IUserConnectionRepository _userConnections;
        private readonly ISessionRepository _repository;

        public MessageHub(ILogger<MessageHub> logger, IUserConnectionRepository userConnections, ISessionRepository repository)
        {
            _logger = logger;
            _userConnections = userConnections;
            _repository = repository;
        }

        [HubMethodName(SignalRClientMessages.ParticipantVoted)]
        public async Task ParticipantVoted(int choiceId)
        {
            var sessionId = Context.User.GetSessionId();
            var username = Context.UserIdentifier;

            var session = await _repository.GetAsync(int.Parse(sessionId));

            session.SetVote(username, choiceId);

            _repository.Update(session);
            var success = await _repository.UnitOfWork.SaveEntitiesAsync();
            if (!success)
            {
                throw new Exception("Unprocessable entity");
            }

            await Clients.Group(sessionId).SendAsync(SignalRServerMessages.VotesUpdated, session.Choices.ToChoicesResponse());
        }

        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.User.GetSessionId();

            
            var prevConnectionId = await _userConnections.RemoveUserConnectionFromGroup(sessionId, Context.UserIdentifier);
            if (!string.IsNullOrEmpty(prevConnectionId))
            {
                await Groups.RemoveFromGroupAsync(prevConnectionId, sessionId);
            }

            await _userConnections.AddUserConnectionToGroup(sessionId, Context.UserIdentifier, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            var activeUsers = await _userConnections.GetUsers(sessionId);

            await Clients.Group(sessionId).SendAsync(SignalRServerMessages.ActiveUsersUpdated, activeUsers);
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

            await Clients.Group(sessionId).SendAsync(SignalRServerMessages.ActiveUsersUpdated, activeUsers);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
