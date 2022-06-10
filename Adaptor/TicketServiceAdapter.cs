using System;

namespace Adapter
{
    public class TicketServiceAdapter: ITicketServiceAdapter
    {
        private readonly ITicketManager _ticketManager;

        public TicketServiceAdapter(SessionManager sessionManager, ITicketManager ticketManager)
        {
            _ticketManager = ticketManager;
        }

        public string CreateSession()
        {
            var sessionToken = Guid.NewGuid().ToString();

            return sessionToken;
        }

        public void ReleaseSession(string sessionToken)
        {

        }

        public void Login(string sessionToken,string userId,string password) => _ticketManager.Login(sessionToken, userId, password);
    }
}
