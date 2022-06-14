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
        const string InvalidSessionToken = "Invalid session token";

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
            SessionCreateResponse result;
            try
            {
                session.UserId = model.User;
                _ticketServiceAdapter.CreateSession(session);

                result = new SessionCreateResponse
                {
                    SessionToken = session.SessionToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("session-release")]
        public IActionResult ReleaseSession(SessionReleaseRequest model)
        {
            try
            {
                _ticketServiceAdapter.ReleaseSession(model.SessionToken);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }

            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequest model)
        {
            try
            {
                _ticketServiceAdapter.Login(model.SessionToken, model.User, model.Password);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("makecall")]
        public IActionResult MakeCallRequest(MakeCallRequest model)
        {
            try
            {
                _ticketServiceAdapter.MakeCall(model.SessionToken, model.PhoneNumber);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }


        [HttpPost]
        [Route("hangup")]
        public IActionResult HangUp(HangUpRequest model)
        {
            try
            {
                _ticketServiceAdapter.HangUp(model.SessionToken, model.SessionToken, model.User);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("outcome")]
        public IActionResult SubmitOutcome(SubmitOutcomeRequest model)
        {
            try
            {
                _ticketServiceAdapter.SubmitOutcome(model.SessionToken, model.Campaign, model.User, model.Outcome);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("callback")]
        public IActionResult SubmitCallback(SubmitCallbackRequest model)
        {
            try
            {
                _ticketServiceAdapter.Callback(model.SessionToken, model.CallDateTime);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("request-break")]
        public IActionResult RequestBreak(BreakRequest model)
        {
            try
            {
                _ticketServiceAdapter.RequestBreak(model.SessionToken, model.Campaign, model.User);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("resume")]
        public IActionResult Resume(BreakRequest model)
        {
            try
            {
                _ticketServiceAdapter.Resume(model.SessionToken, model.Campaign, model.User);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("request-logout")]
        public IActionResult RequestLogout(LogoutRequest model)
        {
            try
            {
                _ticketServiceAdapter.RequestLogout(model.SessionToken, model.Campaign, model.User);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("poll-event")]
        public IActionResult PollEvent(PollEventRequest model)
        {
            EventBase result;
            try
            {
                result = _ticketServiceAdapter.GetEvent(model.SessionToken, model.Campaign, model.User);
            }
            catch (SessionNotFoundException)
            {
                return StatusCode(401, InvalidSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex);
            }

            return Ok(result);
        }
    }
}
