﻿using System;
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
        private readonly IDataManager _dataManager;

        public TicketManager(ITicketServiceClient ticketServiceClient, IDataManager dataManager)
        {
            _ticketServiceClient = ticketServiceClient;
            _dataManager = dataManager;
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
            };

            _ticketServiceClient.AvailableEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "Available",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "UserReady"
                });
            };

            _ticketServiceClient.AgentFreeEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "AgentFree",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "BreakGranted"
                });
            };

            _ticketServiceClient.CallEndedEvent += (s, e) =>
            {
                Enqueue(new StatusChangeEvent()
                {
                    SessionToken = _session.SessionToken,
                    Event = "CallEnded",
                    User = e.UserId,
                    Campaign = e.Campaign,
                    Status = "CallEnded"
                });
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
            _ticketServiceClient.ValidateUser(userId, password);
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
