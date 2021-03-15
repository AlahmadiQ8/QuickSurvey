using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using QuickSurvey.Core.Entities;
using QuickSurvey.Core.Interfaces;

namespace QuickSurvey.Infrastructure.Data
{
    public class InMemorySessionService : ISessionService
    {
        private readonly IList<Session> _sessions = new List<Session>();
        private int _counter = 0;

        public Task<Session> CreateSessionAsync(string name)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            _sessions.Add(session);

            return Task.FromResult(session);
        }

        public Task<Session> GetSessionByIdAsync(Guid id)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(session);
        }

        public Task<bool> DeleteSessionAsync(Session entity)
        {
            return Task.FromResult(_sessions.Remove(entity));
        }

        public Task AddParticipantAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task RemoveParticipantAsync(Participant participant)
        {
            throw new NotImplementedException();
        }

        public Task AddChoiceAsync(Choice choice)
        {
            throw new NotImplementedException();
        }

        public Task RemoveChoiceAsync(Choice choice)
        {
            throw new NotImplementedException();
        }
    }
}
