using System;
using System.Collections.Generic;
using System.Text;

namespace Adapter
{
    public class SessionManager
    {
        string CreateSession()
        {
            var sessionToken = Guid.NewGuid().ToString();

            return sessionToken;
        }

        Session GetSession(string SessionToken)
        {
            return null;
        }
    }
}
