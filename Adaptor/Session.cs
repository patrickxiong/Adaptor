using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Workshop2022.API.Models;

namespace Adapter
{
    public class Session
    {
        public const int SessionLifetimeInMinutes = 20;

        public string SessionToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public UserState State { get; set; }
        public ITicketManager TicketManager { get; set; }

        public Session(ITicketManager ticketManager)
        {
            TicketManager = ticketManager;
            TicketManager.Session = this;
            SessionToken = Guid.NewGuid().ToString();
            State = UserState.SessionCreated;
            ExpireTime = DateTime.Now.AddMinutes(SessionLifetimeInMinutes);
        }

        public void Dispose()
        {
            TicketManager.Disconnect();
        }
    }
}
