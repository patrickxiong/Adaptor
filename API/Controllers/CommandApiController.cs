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

        public CommandApiController(ILogger<CommandApiController> logger, ITicketServiceAdapter ticketServiceAdapter)
        {
            _logger = logger;
            _ticketServiceAdapter = ticketServiceAdapter;
        }


        [HttpGet]
        [Route("service-status")]
        public string GetServiceStatus()
        {
            return "OK";
        }

        [HttpPost]
        [Route("session-create")]
        public IActionResult CreateSession(SessionCreateRequest model, [FromServices] Session session)
        {
            session.UserId = model.User;
            _ticketServiceAdapter.CreateSession(session);

            var result = new SessionCreateResponse
            {
                SessionToken = session.SessionToken
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
            _ticketServiceAdapter.MakeCall(model.SessionToken, model.PhoneNumber);
            return Ok();
        }


        [HttpPost]
        [Route("hangup")]
        public IActionResult HangUp(HangUpRequest model)
        {
            _ticketServiceAdapter.HangUp(model.SessionToken, model.SessionToken, model.User);
            return Ok();
        }

        [HttpPost]
        [Route("outcome")]
        public IActionResult SubmitOutcome(SubmitOutcomeRequest model)
        {
            _ticketServiceAdapter.SubmitOutcome(model.SessionToken, model.Campaign, model.User, model.Outcome);
            return Ok();
        }

        [HttpPost]
        [Route("callback")]
        public IActionResult SubmitCallback(SubmitCallbackRequest model)
        {
            _ticketServiceAdapter.Callback(model.SessionToken, model.CallDateTime);
            return Ok();
        }

        [HttpPost]
        [Route("request-break")]
        public IActionResult RequestBreak(BreakRequest model)
        {
            _ticketServiceAdapter.RequestBreak(model.SessionToken, model.Campaign, model.User);
            return Ok();
        }

        [HttpPost]
        [Route("resume")]
        public IActionResult Resume(BreakRequest model)
        {
            _ticketServiceAdapter.Resume(model.SessionToken, model.Campaign, model.User);
            return Ok();
        }

        [HttpPost]
        [Route("request-logout")]
        public IActionResult RequestLogout(LogoutRequest model)
        {
            _ticketServiceAdapter.RequestLogout(model.SessionToken, model.Campaign, model.User);
            return Ok();
        }


        [HttpPost]
        [Route("poll-event")]
        public IActionResult PollEvent(PollEventRequest model)
        {
            var result = _ticketServiceAdapter.GetEvent(model.SessionToken, model.Campaign, model.User);
            return Ok(result);
        }
    }
}
