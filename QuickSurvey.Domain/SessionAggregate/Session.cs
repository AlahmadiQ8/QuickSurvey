using System;
using System.Collections.Generic;
using System.Linq;
using QuickSurvey.Core.Entities;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.SessionAggregate
{
    public class Session : Entity
    {
        public string Name { get; private set; }
        public IReadOnlyCollection<Participant> Participants => _participants;
        public IReadOnlyCollection<Choice> Choices => _choices;

        private static int MaxNumberOfChoices = 6;

        private readonly List<Participant> _participants;
        private readonly List<Choice> _choices;
        private DateTime _dateCreated;

        protected Session()
        {
            _participants = new List<Participant>();
            _choices = new List<Choice>();
        }

        public Session(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _dateCreated = DateTime.UtcNow;
        }

        public void AddParticipant(string userName)
        {
            var existingParticipant = _participants.SingleOrDefault(p =>
                string.Equals(p.UserName, userName, StringComparison.CurrentCultureIgnoreCase));

            if (existingParticipant != null)
            {
                throw new SessionExceptions($"Participant with username = {userName} already exists");
            }

            var participant = new Participant(userName);
            _participants.Add(participant);
        }

        public void AddChoices(string[] choiceTexts)
        {
            if (choiceTexts.Length > MaxNumberOfChoices)
            {
                throw new SessionExceptions($"Choices cannot exceed a maximum of {MaxNumberOfChoices}");
            }

            foreach (var choice in choiceTexts)
            {
                if (string.IsNullOrEmpty(choice))
                {
                    throw new SessionExceptions("Choice cannot be empty");
                }
            }

            if (choiceTexts.Distinct().Count() != choiceTexts.Length)
            {
                throw new SessionExceptions("Choices cannot have duplicates");
            }

            foreach (var choiceText in choiceTexts)
            {
                _choices.Add(new Choice(choiceText));
            }
        }

        public void SetVote(int participantId, int choiceId)
        {
            var participant = _participants.SingleOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                throw new SessionExceptions($"No participant with id = {participantId} found");
            }

            var choice = _choices.SingleOrDefault(c => c.Id == choiceId);
            if (choice == null)
            {
                throw new SessionExceptions($"No choice with id = {choiceId} found");
;           }

            choice.AddParticipantVote(participant);
        }
    }
}
