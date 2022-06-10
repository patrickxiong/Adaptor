using System;

namespace Adapter
{
    public interface ITicketManager
    {
        void Connect();
        void Disconnect();

        void ValidateUser();
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