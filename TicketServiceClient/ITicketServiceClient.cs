using System;

namespace Workshop2022.TicketServiceClient
{
    public interface ITicketServiceClient
    {
        void Connect(string ip, string port);
        void Disconnect();

        void ValidateUser(string userId, string password);
        void Login(string userId, string extension, string campaign);
        void Available();
        void Unavailable();
        void Logout();

        void MakeCall(string phoneNumber);
        void HangUp();

        void TransactionComplete(int outcome);
        void Callback(DateTime callDateTime);

        event EventHandler<ErrorResponse> ErrorEvent;
        event EventHandler<ValidatedUserResponse> ValidatedUserEvent;
        event EventHandler<LoggedInResponse> LoggedInEvent;
        event EventHandler<LoggedOutResponse> LoggedOutEvent;
        event EventHandler<AvailableResponse> AvailableEvent;
        event EventHandler<AgentFreeResponse> AgentFreeEvent;
        event EventHandler<CallEndedResponse> CallEndedEvent;
        event EventHandler<TicketDataResponse> TicketDataEvent;

    }
}
