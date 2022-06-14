using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Workshop2022.API.Models;
using Workshop2022.TicketServiceClient;

namespace Adapter
{
    public class TicketManager : ITicketManager
    {
        public ConcurrentQueue<EventBase> OutEvents = new ConcurrentQueue<EventBase>();

        private Session _session;

        public Session Session
        {
            get => _session;
            set => _session = value;
        }

        private readonly ITicketServiceClient _ticketServiceClient;

        public TicketManager(ITicketServiceClient ticketServiceClient)
        {
            _ticketServiceClient = ticketServiceClient;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _ticketServiceClient.ErrorEvent += (s, e) =>
            {
                OutEvents.Enqueue(new ErrorEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "Error",
                    User = _session.UserId,
                    Campaign = _session.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    ErrorMessage = $"ErrorCode = {e.ErrorCode}, ErrorMessage = {e.ErrorMessage}"
                });
            };

            _ticketServiceClient.ValidatedUserEvent += (s, e) =>
            {
                //OutEvents.Enqueue(new StatusChangeEvent()
                //{
                //    SessionToken = _session.SessionToken,
                //    Event = "User validated",
                //    User = e.UserId,
                //    Campaign = _session.Campaign,
                //    Expiry = DateTime.Now.AddSeconds(15),
                //    Status = e.IsValidated ? "Validated" : "Not-validated"
                //});



            };

            _ticketServiceClient.LoggedInEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "LoggedIn",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "LoggedIn"
                });
            };

            _ticketServiceClient.LoggedOutEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "LoggedOut",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "LoggedOut"
                });
            };

            _ticketServiceClient.LoggedOutEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "LoggedOut",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "LoggedOut"
                });
            };

            _ticketServiceClient.AvailableEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "Available",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "UserReady"
                });
            };

            _ticketServiceClient.AgentFreeEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "AgentFree",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "BreakGranted"
                });
            };

            _ticketServiceClient.CallEndedEvent += (s, e) =>
            {
                OutEvents.Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "CallEnded",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    Status = "CallEnded"
                });
            };

            _ticketServiceClient.TicketDataEvent += (s, e) =>
            {
                var dataList = new List<DataFields>();
                foreach (var d in e.Data)
                {
                    dataList.Add(new DataFields()
                    {
                        Field = d.FieldName,
                        Type = d.FieldType,
                        Value = d.FieldValue
                    });
                }

                OutEvents.Enqueue(new TicketDataEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "CallEnded",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Expiry = DateTime.Now.AddSeconds(15),
                    PhoneNumber = e.PhoneNumber,
                    Data = dataList
                });
            };
        }

        public void Available()
        {
            _ticketServiceClient.Available();
        }

        public void Callback(DateTime callDateTime)
        {
            _ticketServiceClient.Callback(callDateTime);
        }

        public void Connect()
        {
            _ticketServiceClient.Connect("172.25.12.79", "6502");
            //ConfigurationManager.AppSettings["IP"], ConfigurationManager.AppSettings["Port"]
        }

        public void Disconnect()
        {
            _ticketServiceClient.Disconnect();
        }

        public void HangUp()
        {
            _ticketServiceClient.HangUp();
        }

        public void Login(string userId, string password)
        {
            
            //_ticketServiceClient.Login(userId, extension, campaign);

        }

        public void Logout()
        {
            _ticketServiceClient.Logout();
        }

        public void MakeCall(string phoneNumber)
        {
            _ticketServiceClient.MakeCall(phoneNumber);
        }

        public void TransactionComplete(int outcome)
        {
            _ticketServiceClient.TransactionComplete(outcome);
        }

        public void Unavailable()
        {
            _ticketServiceClient.Unavailable();
        }

        public bool TryPopEvent(out EventBase result) => OutEvents.TryDequeue(out result);
    }
}
