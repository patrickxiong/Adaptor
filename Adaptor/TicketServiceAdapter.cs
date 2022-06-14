using System;
using Workshop2022.API.Models;

namespace Adapter
{
    public class TicketServiceAdapter : ITicketServiceAdapter
    {
        private readonly ISessionManager _sessionManager;

        public TicketServiceAdapter(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void CreateSession(Session session)
        {
            var ticketManager = session.TicketManager;
            ticketManager.Connect();
            _sessionManager.AddSession(session);
        }

        public void ReleaseSession(string sessionToken)
        {
            var session = _sessionManager.GetSession(sessionToken);
            if (session == null)
                throw new Exception("No session found!");

            var ticketManager = session.TicketManager;
            ticketManager.Disconnect();
            _sessionManager.ReleaseSession(sessionToken);
        }

        public void Login(string sessionToken, string userId, string password)
        {
            var session = _sessionManager.GetSession(sessionToken);
            if (session == null)
                throw new Exception("No session found!");

            var ticketManager = session.TicketManager;
            ticketManager.Login(userId, password);
        }

        public EventBase GetEvent(string sessionToken, string campaign, string user)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            if (ticketManager.TryPopEvent(out var result))
            {
                return result;
            }

            return default;
        }

        public void RequestLogout(string sessionToken, string campaign, string user)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            ticketManager.Logout();
        }

        public void Resume(string sessionToken, string campaign, string user)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            ticketManager.Available();
        }

        public void RequestBreak(string sessionToken, string campaign, string user)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            ticketManager.Unavailable();
        }

        public void SubmitOutcome(string sessionToken, string campaign, string user, int outcome)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            ticketManager.TransactionComplete(outcome);
        }

        private (Session, ITicketManager) GetSessionAndTicketManager(string sessionToken)
        {
            var session = _sessionManager.GetSession(sessionToken);
            if (session == null)
                throw new Exception("No session found!");

            var ticketManager = session.TicketManager;
            return (session, ticketManager);
        }

        public void HangUp(string sessionToken, string campaign, string user)
        {
            var (_, ticketManager) = GetSessionAndTicketManager(sessionToken);
            ticketManager.HangUp();
        }
    }
}
