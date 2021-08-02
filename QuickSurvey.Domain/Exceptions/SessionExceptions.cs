using System;

namespace QuickSurvey.Core.Exceptions
{
    public class SessionExceptions : Exception
    {
        public SessionExceptions()
        {
        }

        public SessionExceptions(string? message) : base(message)
        {
        }
    }
}
