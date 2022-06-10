using System;

namespace Adapter
{
    public class TicketServiceAdapter
    {
        public TicketServiceAdapter(SessionManager sessionManager)
        {

        }

        public string CreateSession()
        {
            var sessionToken = Guid.NewGuid().ToString();

            return sessionToken;
        }

        public void ReleaseSession(string SessionToken)
        {

        }

        public void Login(string SessionToken,string UserId,string Password)
        {

        }
    }
}
