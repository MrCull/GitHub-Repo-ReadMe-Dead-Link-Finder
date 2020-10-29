using System;

namespace WebsiteLinksChecker
{
    public class TooManyRequestsLoopsException : Exception
    {
        public TooManyRequestsLoopsException(string message) : base(message)
        {
        }
    }
}
