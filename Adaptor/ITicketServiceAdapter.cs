using Workshop2022.API.Models;

namespace Adapter
{
    public interface ITicketServiceAdapter
    {
        void Login(string modelSessionToken, string modelUser, string modelPassword);

        void ReleaseSession(string modelSessionToken);

        void CreateSession(Session session);

        EventBase GetEvents(string sessionToken, string campaign, string user);

        void RequestLogout(string sessionToken, string campaign, string user);

        void Resume(string sessionToken, string campaign, string user);

        void RequestBreak(string sessionToken, string campaign, string user);

        void SubmitOutcome(string sessionToken, string campaign, string user, string outcome);

        void HangUp(string sessionToken, string campaign, string user);
    }
}