using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Adapter
{
    public class SessionManager: ISessionManager
    {
        private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

        public SessionManager(ILogger<SessionManager> logger)
        {
        }
        public void AddSession(Session session)
        {
            KillDeadSessions();

            _sessions.Add(session.SessionToken, session);
        }

        public Session GetSession(string sessionToken)
        {
            if (_sessions.TryGetValue(sessionToken, out var session))
            {
                if (session.ExpireTime < DateTime.Now)
                    session = null;
                else
                    session.ExpireTime = DateTime.Now.AddMinutes(Session.SessionLifetimeInMinutes);

                if (session?.State == UserState.SessionReleased)
                    session = null;
            }

            return session;
        }

        public void ReleaseSession(string sessionToken)
        {
            _sessions.Remove(sessionToken);
        }

        private void KillDeadSessions()
        {
            foreach (var session in _sessions.Values.ToArray() )
            {
                if (session.ExpireTime < DateTime.Now)
                    ReleaseSession(session.SessionToken);
            }
        }
    }
}
