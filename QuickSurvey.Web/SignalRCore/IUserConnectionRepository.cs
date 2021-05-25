using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickSurvey.Web.SignalRCore
{
    public interface IUserConnectionRepository
    {
        Task AddUserConnectionToGroup(string group, string user, string connectionId);
        Task<string> RemoveUserConnectionFromGroup(string group, string user);
        Task<List<string>> GetUsers(string group);
    }
}
