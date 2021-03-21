using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickSurvey.Core.SeedWork;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository, IDisposable
    {
        private readonly SurveyContext _context;

        public SessionRepository(SurveyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;
        public Session Add(Session session)
        {
            return _context.Sessions.Add(session).Entity;
        }

        public void Update(Session session)
        {
            _context.Entry(session).State = EntityState.Modified;
        }

        public async Task<Session> GetAsync(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => sessionId == s.Id);
            if (session == null)
            {
                session = _context.Sessions.Local.FirstOrDefault(s => sessionId == s.Id);
            }

            return session;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
