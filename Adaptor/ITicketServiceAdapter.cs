namespace Adapter
{
    public interface ITicketServiceAdapter
    {
        void Login(string modelSessionToken, string modelUser, string modelPassword);

        void ReleaseSession(string modelSessionToken);

        string CreateSession(string user);
    }
}