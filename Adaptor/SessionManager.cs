using System;
using System.Collections.Generic;
using System.Text;

namespace Adapter
{
    public class SessionManager
    {
        private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

        string CreateSession()
        {
            var sessionToken = Guid.NewGuid().ToString();
            var session = new Session
            {
                SessionToken = sessionToken
            };

            _sessions.Add(sessionToken, session);

            return sessionToken;
        }

        Session GetSession(string sessionToken)
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

        void ReleaseSession(string sessionToken)
        {
            _sessions.Remove(sessionToken);
        }
    }
}
