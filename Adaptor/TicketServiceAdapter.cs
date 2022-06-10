using Workshop2022.API.Models;

namespace Adapter
{
    public class TicketServiceAdapter: ITicketServiceAdapter
    {
        private readonly ITicketManager _ticketManager;
        private readonly ISessionManager _sessionManager;

        public TicketServiceAdapter(ISessionManager sessionManager, ITicketManager ticketManager)
        {
            _sessionManager = sessionManager;
            _ticketManager = ticketManager;
        }

        public void CreateSession(Session session) => _sessionManager.AddSession(session);

        public void ReleaseSession(string sessionToken) => _sessionManager.ReleaseSession(sessionToken);

        public void Login(string sessionToken,string userId,string password) => _ticketManager.Login(sessionToken, userId, password);

        public EventBase GetEvents(string sessionToken, string campaign, string user)
        {
            return default;
        }
    }
}
