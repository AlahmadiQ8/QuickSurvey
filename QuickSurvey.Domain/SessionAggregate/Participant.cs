using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.SessionAggregate
{
    public class Participant : Entity
    {
        public string Username => _username;
        public int SessionId { get; private set; }

        private readonly string _username;

        public Participant(string userName)
        {
            _username = userName;
        }

        protected Participant()
        {
        }
    }
}