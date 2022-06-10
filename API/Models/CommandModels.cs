using System;

namespace Workshop2022.API.Models
{
    public class LoginRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class MakeCallRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class HangUpRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class SubmitOutcomeRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
        public int Outcome { get; set; }
    }

    public class SubmitCallbackRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
        public DateTime CallDateTime { get; set; }
    }

    public class BreakRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class ResumeRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class LogoutRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class PollEventRequest
    {
        public string SessionToken { get; set; }
        public string User { get; set; }
        public string Campaign { get; set; }
    }
}
