using System;

namespace Adapter
{
    public interface ITicketManager
    {
        Session Session { get; set; }
        void Connect();
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
    }
}