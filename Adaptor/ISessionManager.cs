namespace Adapter
{
    public interface ISessionManager
    {
        string CreateSession();

        void ReleaseSession(string sessionToken);
    }
}