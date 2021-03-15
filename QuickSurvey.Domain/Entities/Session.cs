using System;
using System.Collections.Generic;

namespace QuickSurvey.Core.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<Participant> Participants { get; set; } = new List<Participant>();
        public IList<Choice> Choices { get; set; } = new List<Choice>();
    }
}
