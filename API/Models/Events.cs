using System;
using System.Collections.Generic;

namespace Workshop2022.API.Models
{
    public class EventBase
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
        public DateTime Expiry { get; set; }
        
        public string Event { get; set; }
    }
    public class StatusChangeEvent : EventBase
    {
        public string Status { get; set; }
    }

    public class ErrorEvent : EventBase
    {
        public string ErrorMessage { get; set; }
    }

    public class TicketDataEvent : EventBase
    {
        public string PhoneNumber { get; set; }
        public IReadOnlyList<DataFields> Data { get; set; }
    }

    public class DataFields
    {
        public string Field { get; set; }

        public string Type { get; set; }   // JS/TS data types

        public object Value { get; set; }
    }
}
