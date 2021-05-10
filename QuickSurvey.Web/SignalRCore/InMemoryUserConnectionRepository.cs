using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickSurvey.Web.SignalRCore;

namespace QuickSurvey.Infrastructure.Repositories
{
    class InMemoryUserConnectionRepository : IUserConnectionRepository
    {
        private readonly IDictionary<string, IDictionary<string, string>> _userConnections = new Dictionary<string, IDictionary<string, string>>();

        public Task AddUserConnectionToGroup(string group, string user, string connectionId)
        {
            if (!_userConnections.ContainsKey(group))
                _userConnections[group] = new Dictionary<string, string>();

            _userConnections[group][user] = connectionId;
            return Task.CompletedTask;
        }

        public Task<string> RemoveUserConnectionFromGroup(string group, string user)
        {
            var currentConnectionId = _userConnections[group][user];
            _userConnections[group].Remove(user);
            return Task.FromResult(currentConnectionId);
        }

        public Task<bool> UserInGroup(string group, string user)
        {
            if (_userConnections.ContainsKey(group)) 
                return Task.FromResult(_userConnections[group].ContainsKey(user));
            return Task.FromResult(false);
        }

        public Task<List<string>> GetUsers(string group)
        {
            if (_userConnections.ContainsKey(group))
                return Task.FromResult(_userConnections[group].Keys.ToList());

            throw new NotImplementedException();
        }
    }
}
