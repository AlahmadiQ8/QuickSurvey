using System.Collections.Generic;

namespace QuickSurvey.Core.Entities
{
    public class Choice
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IList<Participant> Participants { get; set; }
    }
}