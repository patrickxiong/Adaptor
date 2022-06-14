using System;

namespace Adapter
{
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException(string noSessionFound):base(noSessionFound)
        {
        }
    }
}