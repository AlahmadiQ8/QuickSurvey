using System;
using System.Threading.Tasks;
using QuickSurvey.Core.Entities;

namespace QuickSurvey.Core.Interfaces
{
    public interface ISessionService
    {
        Task<Session> CreateSessionAsync(string name);
        Task<Session> GetSessionByIdAsync(Guid id);
        public Task<bool> DeleteSessionAsync(Session entity);
        Task AddParticipantAsync(string username);
        Task RemoveParticipantAsync(Participant participant);
        Task AddChoiceAsync(Choice choice);
        Task RemoveChoiceAsync(Choice choice);
    }
}
