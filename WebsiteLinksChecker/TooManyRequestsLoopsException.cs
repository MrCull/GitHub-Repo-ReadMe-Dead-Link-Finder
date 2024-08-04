using System;

namespace LinksChecker
{
    public class TooManyRequestsLoopsException : Exception
    {
        public TooManyRequestsLoopsException(string message) : base(message)
        {
        }
    }
}
