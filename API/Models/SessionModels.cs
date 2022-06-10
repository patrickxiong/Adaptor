namespace Workshop2022.API.Models
{
    public class SessionCreateRequest
    {
        public string User { get; set; }
    }

    public class SessionCreateResponse
    {
        public string SessionToken { get; set; }
    }

    public class SessionReleaseRequest
    {
        public string SessionToken { get; set; }
    }

}
