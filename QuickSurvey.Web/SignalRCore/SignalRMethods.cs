using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickSurvey.Web.SignalRCore
{
    public static class SignalRServerMessages
    {
        public const string ActiveUsersUpdated = "ActiveUsersUpdated";
        public const string VotesUpdated = "VotesUpdated";
        public const string UserAdded = "UserAdded";
    }

    public static class SignalRClientMessages
    {
        public const string ParticipantVoted = "ParticipantVoted";
    }
}
