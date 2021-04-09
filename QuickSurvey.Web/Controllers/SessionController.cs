using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Web.Controllers.Requests;
using QuickSurvey.Web.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuickSurvey.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionRepository _repository;
        private readonly LinkGenerator _linkGenerator;

        public SessionController(ISessionRepository repository, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        // GET: api/<SessionController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SessionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

        // PUT api/<SessionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SessionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
