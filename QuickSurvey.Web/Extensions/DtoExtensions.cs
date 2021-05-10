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
            var choices = session.Choices.Select(c => new ChoiceResponse(c.Id, c.Text, c.Voters.ToList())).ToList();
            var sessionResponse = new SessionResponse(session.Name, choices, session.Participants.Select(p => p.Username).ToList());
            return sessionResponse;
        }

        public static Session ToSessionAggregate(this CreateSurveyRequest request)
        {
            var session = new Session(request.Title);
            session.AddChoices(request.Choices);
            return session;
        } 
    }
}
