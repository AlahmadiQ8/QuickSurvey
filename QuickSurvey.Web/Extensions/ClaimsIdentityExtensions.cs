using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickSurvey.Web.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetSessionId(this ClaimsPrincipal user)
        {
            return user.FindFirst("SessionId")?.Value;
        }
    }
}
