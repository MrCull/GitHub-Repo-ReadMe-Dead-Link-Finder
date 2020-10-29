using System;

namespace WebsiteLinksChecker
{
    public class ElementIdNotFoundException : Exception
    {
        public ElementIdNotFoundException(string message) : base(message)
        {
        }
    }
}
