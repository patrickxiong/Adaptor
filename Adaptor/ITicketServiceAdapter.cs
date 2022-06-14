using System;
using Workshop2022.API.Models;

namespace Adapter
{
    public interface ITicketServiceAdapter
    {
        void Login(string modelSessionToken, string modelUser, string modelPassword);

        void ReleaseSession(string modelSessionToken);

        void CreateSession(Session session);

        EventBase GetEvent(string sessionToken, string campaign, string user);

        void RequestLogout(string sessionToken, string campaign, string user);

        void Resume(string sessionToken, string campaign, string user);

        void RequestBreak(string sessionToken, string campaign, string user);

        void SubmitOutcome(string sessionToken, string campaign, string user, int outcome);

        void HangUp(string sessionToken, string campaign, string user);
        
        void Callback(string sessionToken, DateTime dateTime);

        void MakeCall(string sessionToken, string phoneNumber);
    }
}