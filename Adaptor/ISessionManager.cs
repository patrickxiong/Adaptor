namespace Adapter
{
    public interface ISessionManager
    {
        void AddSession(Session session);

        void ReleaseSession(string sessionToken);
    }
}