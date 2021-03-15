using System;
using System.Threading.Tasks;
using QuickSurvey.Core.Entities;

namespace QuickSurvey.Core.Interfaces
{
    public interface ISessionService
    {
        Task<Session> GetByIdAsync(Guid id);
        Task DeleteAsync(Session entity);
        Task AddParticipant(Participant participant);
        Task RemoveParticipant(Participant participant);
        Task AddChoice(Choice choice);
        Task RemoveChoice(Choice choice);
    }
}
