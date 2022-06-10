namespace Adapter
{
    public interface ISessionManager
    {
        void AddSession(Session session);
        Session GetSession(string sessionToken);

        void ReleaseSession(string sessionToken);
    }
}