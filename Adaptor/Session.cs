using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Workshop2022.API.Models;

namespace Adapter
{
    public class Session : IDisposable
    {
        public string SessionToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public UserState State { get; set; }
        public ITicketManager TicketManager { get; set; }

        public Session(ITicketManager ticketManager)
        {
            TicketManager = ticketManager;
            SessionToken = Guid.NewGuid().ToString();
            State = UserState.SessionCreated;
        }

        public void Dispose()
        {
            //Todo: Dispose TicketManager
        }
    }
}
