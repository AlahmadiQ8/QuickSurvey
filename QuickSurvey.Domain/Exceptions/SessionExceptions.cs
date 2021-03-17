using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSurvey.Core.Exceptions
{
    internal class SessionExceptions : Exception
    {
        public SessionExceptions()
        {
        }

        public SessionExceptions(string? message) : base(message)
        {
        }
    }
}
