using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
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

        public SessionController(ISessionRepository repository, LinkGenerator linkGenerator, ILogger<SessionController> logger)
        {
            _repository = repository;
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        // GET api/<SessionController>/5
        [HttpGet("{sessionId:int}/user/{username}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int sessionId)
        {
            
            var session = await _repository.GetAsync(sessionId);
            var user = (ClaimsIdentity) User.Identity;
            _logger.LogInformation(user.Name);
            _logger.LogInformation(user.FindFirst("SessionId").Value);

            if (session == null)
                return NotFound();
            return Ok(session.ToSessionResponse());
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
            return Created(link, request);
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
