using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickSurvey.Web.Controllers.Responses
{
    public class SessionResponse
    {
        public SessionResponse(string title, IList<ChoiceResponse> choices, IList<string> participants)
        {
            Title = title;
            Choices = choices;
            Participants = participants;
        }

        public string Title { get; }
        public IList<ChoiceResponse> Choices { get; }
        public IList<string> Participants { get; }

    }

    public class ChoiceResponse
    {
        public ChoiceResponse(int id, string text, IList<string> voters)
        {
            Id = id;
            Text = text;
            Voters = voters;
        }
        public string Text { get; }
        public int Id { get; }
        public IList<string> Voters { get; }
    }
}
