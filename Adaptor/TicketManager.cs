using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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
        private readonly IDataManager _dataManager;
        private IConfiguration _configuration;

        public TicketManager(ITicketServiceClient ticketServiceClient, IDataManager dataManager, IConfiguration configuration)
        {
            _ticketServiceClient = ticketServiceClient;
            _dataManager = dataManager;
            _configuration = configuration;
            SubscribeToEvents();
        }

        private void Enqueue(EventBase @event)
        {
            @event.Expiry=DateTime.Now.AddMinutes(15);
            OutEvents.Enqueue(@event);
        }

        private void SubscribeToEvents()
        {
            _ticketServiceClient.ErrorEvent += (s, e) =>
            {
                Enqueue(new ErrorEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "Error",
                    User = _session.UserId,
                    Campaign = _session.Campaign,
                    ErrorMessage = $"ErrorCode = {e.ErrorCode}, ErrorMessage = {e.ErrorMessage}"
                });
            };

            _ticketServiceClient.ValidatedUserEvent += (s, e) =>
            {
                if (e.IsValidated)
                {
                    _ticketServiceClient.Login(Session.UserId, "2004", _dataManager.GetCampaign(Session.UserId));
                }
                else
                {
                    Enqueue(new ErrorEvent()
                    {
                        SessionToken = _session.SessionToken,
                        Event = "Error",
                        User = e.UserId,
                        Campaign = _dataManager.GetCampaign(Session.UserId),
                        ErrorMessage = e.ErrorMessage
                    });
                }
            };

            _ticketServiceClient.LoggedInEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "LoggedIn",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "LoggedIn"
                });

                Session.State = UserState.LoggedIn;
                _ticketServiceClient.Available();
            };

            _ticketServiceClient.LoggedOutEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "LoggedOut",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "LoggedOut"
                });
                Session.State = UserState.LoggedOut;
            };

            _ticketServiceClient.AvailableEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "Available",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "Ready"
                });
                Session.State = UserState.Ready;
            };

            _ticketServiceClient.AgentFreeEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "OnBreak",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "OnBreak"
                });

                Session.State = UserState.OnBreak;
            };

            _ticketServiceClient.CallEndedEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "OffCall",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "OffCall"
                });

                Session.State = UserState.OffCall;
            };

            _ticketServiceClient.TicketDataEvent += (s, e) =>
            {
                var dataList = new List<DataFields>();
                if (e.Data != null)
                    foreach (var d in e.Data)
                    {
                        dataList.Add(new DataFields()
                        {
                            Field = d.FieldName,
                            Type = d.FieldType,
                            Value = d.FieldValue
                        });
                    }
                Enqueue(new TicketDataEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "TicketData",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    PhoneNumber = e.PhoneNumber,
                    Data = dataList
                });

                if (e.IsActiveCall)
                {
                    Enqueue(new StatusChangeEvent
                    {
                        SessionToken = _session.SessionToken,
                        Event = "OnCall",
                        User = e.UserId,
                        Campaign = e.Campaign,
                        Status = "OnCall"
                    });

                    Session.State = UserState.OnCall;
                }
                //else
                //if (e.IsManualCall && Session.State== UserState.Dialling)
                //{
                //    Enqueue(new StatusChangeEvent
                //    {
                //        SessionToken = _session.SessionToken,
                //        Event = "OnCall",
                //        User = e.UserId,
                //        Campaign = e.Campaign,
                //        Status = "OnCall"
                //    });
                //    Session.State = UserState.OnCall;
                //}

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
            var ip = _configuration["ServiceHost:ip"];
            var port = _configuration["ServiceHost:port"];
            _ticketServiceClient.Connect(ip, port);
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
            _ticketServiceClient.ValidateUser(userId, password);
        }

        public void Logout()
        {
            _ticketServiceClient.Logout();
        }

        public void MakeCall(string phoneNumber)
        {
            Enqueue(new StatusChangeEvent
            {
                SessionToken = _session.SessionToken,
                Event = "Dialling",
                User = Session.UserId,
                Campaign = Session.Campaign,
                Status = "Dialling"

            });
            Session.State = UserState.Dialling;

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

        public bool TryPopEvent(out EventBase result)
        {
            while (OutEvents.TryDequeue(out result))
            {
                if (result.Expiry >= DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            _ticketServiceClient?.Logout();
            _ticketServiceClient?.Disconnect();
        }
    }
}
