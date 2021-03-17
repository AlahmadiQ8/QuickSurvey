using System.Collections.Generic;
using System.Linq;
using QuickSurvey.Core.Entities;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.SessionAggregate
{
    public class Choice : Entity
    {
        public string Text { get; }
        public IReadOnlyCollection<Participant> Participants => _participants;

        private readonly List<Participant> _participants;

        public Choice(string text)
        {
            Text = text;
        }

        protected Choice()
        {
            _participants = new List<Participant>();
        }

        public void AddParticipantVote(Participant participant)
        {
            var existingParticipant = _participants.SingleOrDefault(p => p == participant);
            if (existingParticipant != null)
            {
                throw new SessionExceptions($"Participant {participant.UserName} already voted to {Text}");
            }
            _participants.Add(participant);
        }
    }
}