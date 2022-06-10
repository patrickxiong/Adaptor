using System;
using Adapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Workshop2022.API.Models;

namespace Workshop2022.API.Controllers
{
    [ApiController]
    [Route("API")]
    public class CommandApiController : ControllerBase
    {
        private const int EVENT_EXPIRY_TIME_SECONDS = 15;

        private readonly ILogger<CommandApiController> _logger;
        private readonly ITicketServiceAdapter _ticketServiceAdapter;
        private readonly ISessionManager _sessionManager;


        public CommandApiController(ILogger<CommandApiController> logger, ITicketServiceAdapter ticketServiceAdapter, ISessionManager sessionManager)
        {
            _logger = logger;
            _ticketServiceAdapter = ticketServiceAdapter;
            _sessionManager = sessionManager;
        }


        [HttpGet]
        [Route("service-status")]
        public string GetServiceStatus()
        {
            return "OK";
        }

        [HttpPost]
        [Route("session-create")]
        public IActionResult CreateSession(SessionCreateRequest model, Session session)
        {
            var generatedToken = _ticketServiceAdapter.CreateSession(model.User);
            var result = new SessionCreateResponse
            {
                SessionToken = generatedToken
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("session-release")]
        public IActionResult ReleaseSession(SessionReleaseRequest model)
        {
            _ticketServiceAdapter.ReleaseSession(model.SessionToken);
            return Ok();
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequest model)
        {
            _ticketServiceAdapter.Login(model.SessionToken, model.User, model.Password);
            return Ok();
        }

        [HttpPost]
        [Route("makecall")]
        public IActionResult MakeCallRequest(MakeCallRequest model)
        {

            // -- add code as needed

            return Ok();
        }


        [HttpPost]
        [Route("hangup")]
        public IActionResult HangUp(HangUpRequest model)
        {

            // -- add code as needed

            return Ok();
        }

        [HttpPost]
        [Route("outcome")]
        public IActionResult SubmitOutcome(SubmitOutcomeRequest model)
        {

            // -- add code as needed

            return Ok();
        }

        [HttpPost]
        [Route("callback")]
        public IActionResult SubmitCallback(SubmitCallbackRequest model)
        {

            // -- add code as needed

            return Ok();
        }

        [HttpPost]
        [Route("request-break")]
        public IActionResult RequestBreak(BreakRequest model)
        {

            // -- add code as needed

            return Ok();
        }

        [HttpPost]
        [Route("resume")]
        public IActionResult Resume(BreakRequest model)
        {

            // -- add code as needed

            return Ok();
        }

        [HttpPost]
        [Route("request-logout")]
        public IActionResult RequestLogout(LogoutRequest model)
        {

            // -- add code as needed

            return Ok();
        }


        [HttpPost]
        [Route("poll-event")]
        public IActionResult PollEvent(PollEventRequest model)
        {

            


            // retrieves and return queued events
            var result = new EventBase()  // replace with a respective event(s)
            {
                // DEMO ONLY!!!
                Expiry = DateTime.Now.AddSeconds(EVENT_EXPIRY_TIME_SECONDS)
            };

            return Ok(result);
        }

    }
}
