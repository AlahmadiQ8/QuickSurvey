using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Logging;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Web.Authentication;
using QuickSurvey.Web.Controllers.Requests;
using QuickSurvey.Web.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuickSurvey.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly ISessionRepository _repository;
        private readonly LinkGenerator _linkGenerator;
        private readonly BasicObfuscator _obfuscator;

        public SessionController(ISessionRepository repository, LinkGenerator linkGenerator, ILogger<SessionController> logger, BasicObfuscator obfuscator)
        {
            _repository = repository;
            _linkGenerator = linkGenerator;
            _logger = logger;
            _obfuscator = obfuscator;
        }

        // GET api/<SessionController>/5
        [HttpGet("{sessionId:int}/user/{username}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int sessionId)
        {
            
            var session = await _repository.GetAsync(sessionId);
            var user = (ClaimsIdentity) User.Identity;
            _logger.LogInformation(user.Name);
            _logger.LogInformation(user.FindFirst("SessionId")!.Value);

            if (session == null)
                return NotFound();
            return Ok(session.ToSessionResponse());
        }

        [HttpPost("{sessionId:int}/user/{username}/vote/{choiceId:int}")]
        [Authorize]
        public async Task<IActionResult> SetVote([FromRoute] int sessionId, [FromRoute] int choiceId)
        {
            var session = await _repository.GetAsync(sessionId);
            var user = ((ClaimsIdentity)User.Identity)!.Name;

            try
            {
                session.SetVote(user, choiceId);
            }
            catch (SessionExceptions e)
            {
                return BadRequest(e.Message);
            }

            _repository.Update(session);
            var success = await _repository.UnitOfWork.SaveEntitiesAsync();
            if (!success)
            {
                return UnprocessableEntity();
            }

            return NoContent();
        }

        // POST api/<SessionController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateSurveyRequest request)
        {
            var session = _repository.Add(request.ToSessionAggregate());
            var success = await _repository.UnitOfWork.SaveEntitiesAsync();
            if (!success)
            {
                return UnprocessableEntity();
            }

            var link = _linkGenerator.GetPathByPage("/JoinSession", null, new { Id = session.Id.ToString()});
            return Created(link, null);
        }

        [HttpPost("{sessionId:int}/user/{username}/join")]
        public async Task<ActionResult> Post([FromRoute] int sessionId, [FromRoute] string username)
        {
            var session = await _repository.GetAsync(sessionId);
            try
            {
                session.AddParticipant(username);
            }
            catch (SessionExceptions e)
            {
                ModelState.AddModelError(nameof(username), e.Message);
                return BadRequest();
            }

            await _repository.UnitOfWork.SaveEntitiesAsync();

            _logger.LogDebug($"Participant {username}` is joining session id {sessionId}");

            var accessToken = _obfuscator.Obfuscate(sessionId, username);

            return Created(
                $"/App/PollSession/{sessionId}/Username/{username}?access_token={accessToken}", 
                new { Id = sessionId, Username = username,  AccessToken = accessToken});
        }

        //// PUT api/<SessionController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<SessionController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
