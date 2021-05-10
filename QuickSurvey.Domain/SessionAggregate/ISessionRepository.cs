using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.SessionAggregate
{
    public interface ISessionRepository : IRepository
    {
        Session Add(Session session);
        void Update(Session session);
        Task<Session> GetAsync(int sessionId);
    }
}
