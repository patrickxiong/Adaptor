using Workshop2022.API.Models;

namespace Adapter
{
    public interface ITicketServiceAdapter
    {
        void Login(string modelSessionToken, string modelUser, string modelPassword);

        void ReleaseSession(string modelSessionToken);

        string CreateSession(Session session);

        EventBase GetEvents(string sessionToken, string campaign, string user);
    }
}