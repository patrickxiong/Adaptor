using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Workshop2022.API.Connector;
using Workshop2022.API.Models;

namespace Adapter
{
    public class TicketManager:ITicketManager
    {
        public ConcurrentQueue<EventBase> OutEvents = new ConcurrentQueue<EventBase>();

        private Session _session;
        private ITicketServiceClient _ticketServiceClient;
        public TicketManager(Session session)
        {
          _session = session;
        }


        event EventHandler<ErrorResponse> ErrorEvent;
        event EventHandler<ValidatedUserResponse> ValidatedUserEvent;
        event EventHandler<LoggedInResponse> LoggedInEvent;
        event EventHandler<LoggedOutResponse> LoggedOutEvent;
        event EventHandler<AvailableResponse> AvailableEvent;
        event EventHandler<AgentFreeResponse> AgentFreeEvent;
        event EventHandler<CallEndedResponse> CallEndedEvent;
        event EventHandler<TicketDataResponse> TicketDataEvent;

        public void Available()
        {
            throw new NotImplementedException();
        }

        public void Callback(DateTime callDateTime)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void HangUp()
        {
            throw new NotImplementedException();
        }

        public void Login(string userId, string extension, string campaign)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void MakeCall(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public void TransactionComplete(int outcome)
        {
            throw new NotImplementedException();
        }

        public void Unavailable()
        {
            throw new NotImplementedException();
        }

        public void ValidateUser()
        {
            throw new NotImplementedException();
        }
    }
}
