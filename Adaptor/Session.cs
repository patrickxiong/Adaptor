using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Workshop2022.API.Models;

namespace Adapter
{
    public class Session
    {
        public string SessionToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public UserState State { get; set; }
        public ConcurrentQueue<EventBase> OutEvents = new ConcurrentQueue<EventBase>();
    }
}
