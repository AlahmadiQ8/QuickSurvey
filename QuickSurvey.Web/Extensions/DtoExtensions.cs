using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Web.Controllers.Requests;
using QuickSurvey.Web.Controllers.Responses;

namespace QuickSurvey.Web.Extensions
{
    public static class DtoExtensions
    {
        public static SessionResponse ToSessionResponse(this Session session)
        {
            var choices = session.Choices.ToChoicesResponse();
            var sessionResponse = new SessionResponse(session.Name, choices, session.Participants.Select(p => p.Username).ToList());
            return sessionResponse;
        }

        public static Session ToSessionAggregate(this CreateSurveyRequest request)
        {
            var session = new Session(request.Title);
            session.AddChoices(request.Choices);
            return session;
        }

        public static List<ChoiceResponse> ToChoicesResponse(this IReadOnlyCollection<Choice> choices)
        {
            return choices.Select(c => new ChoiceResponse(c.Id, c.Text, c.Voters.ToList())).ToList();
        }
    }
}
