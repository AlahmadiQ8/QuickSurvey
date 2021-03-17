using System;
using QuickSurvey.Core.SeedWork;

namespace QuickSurvey.Core.Entities
{
    public class Participant : Entity
    {
        public string UserName { get; }

        public Participant(string userName)
        {
            UserName = userName;
        }

        protected Participant()
        {
        }
    }
}