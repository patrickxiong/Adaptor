using System;
using Workshop2022.API.Models;

namespace Adapter
{
    public class TicketServiceAdapter : ITicketServiceAdapter
    {
        private readonly ITicketManager _ticketManager;
        private readonly ISessionManager _sessionManager;

        public TicketServiceAdapter(ISessionManager sessionManager, ITicketManager ticketManager)
        {
            _sessionManager = sessionManager;
            _ticketManager = ticketManager;
        }

        public void CreateSession(Session session)
        {
            _ticketManager.Connect();
            _sessionManager.AddSession(session);

        }

        public void ReleaseSession(string sessionToken)
        {
            _ticketManager.Disconnect();
            _sessionManager.ReleaseSession(sessionToken);
        } 

        public void Login(string sessionToken, string userId, string password)
        {
            var session = _sessionManager.GetSession(sessionToken);
            if (session == null)
                throw new Exception("No session found!");

            _ticketManager.Login(userId, password);
        }

        public EventBase GetEvents(string sessionToken, string campaign, string user)
        {
            return default;
        }

        public void RequestLogout(string sessionToken, string campaign, string user)
        {

        }

        public void Resume(string sessionToken, string campaign, string user)
        {

        }

        public void RequestBreak(string sessionToken, string campaign, string user)
        {

        }

        public void SubmitOutcome(string sessionToken, string campaign, string user, string outcome)
        {
        }

        public void HangUp(string sessionToken, string campaign, string user)
        {

        }
    }
}
