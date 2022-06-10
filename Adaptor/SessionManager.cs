using System;
using System.Collections.Generic;

namespace Adapter
{
    public class SessionManager: ISessionManager
    {
        private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

        public string CreateSession()
        {
            var sessionToken = Guid.NewGuid().ToString();
            var session = new Session
            {
                SessionToken = sessionToken
            };

            _sessions.Add(sessionToken, session);

            return sessionToken;
        }

        public Session GetSession(string sessionToken)
        {
            if (_sessions.TryGetValue(sessionToken, out var session))
            {
                if (session.ExpireTime < DateTime.Now)
                    session = null;
                else
                    session.ExpireTime = DateTime.Now.AddSeconds(15);

                if (session?.State == UserState.SessionReleased)
                    session = null;
            }

            return session;
        }

        public void ReleaseSession(string sessionToken)
        {
            _sessions.Remove(sessionToken);
        }
    }
}
