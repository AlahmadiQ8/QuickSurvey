using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuickSurvey.Core.Entities;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.SessionAggregate
{
    public class Choice : Entity
    {
        public string Text { get; }
        public IReadOnlyCollection<string> Voters => _voters;
        public int SessionId { get; private set; }

        private readonly List<string> _voters;
        private static Regex _regex = new Regex("\\w+");

        public Choice(string text) : this()
        {
            if (string.IsNullOrEmpty(text)) throw new SessionExceptions("Choice cannot be null");
            if (!_regex.IsMatch(text))
            {
                throw new SessionExceptions("Choice can only contain a-z, A-Z, 0-9 and _");
            }
            Text = text;
        }

        protected Choice()
        {
            _voters = new List<string>();
        }

        public void AddParticipantVote(string username)
        {
            var existingParticipant = _voters.SingleOrDefault(p => p == username);
            if (existingParticipant != null)
            {
                throw new SessionExceptions($"Participant {username} already voted to {Text}");
            }
            _voters.Add(username);
        }
    }
}