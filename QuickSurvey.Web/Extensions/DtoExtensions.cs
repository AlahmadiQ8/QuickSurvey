using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Web.Controllers.Requests;

namespace QuickSurvey.Web.Extensions
{
    public static class DtoExtensions
    {
        public static Session ToSessionAggregate(this CreateSurveyRequest request)
        {
            var session = new Session(request.Title);
            session.AddChoices(request.Choices);
            return session;
        } 
    }
}
