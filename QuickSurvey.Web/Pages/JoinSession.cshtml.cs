using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using QuickSurvey.Core.Exceptions;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Web.Pages
{
    public class JoinSessionModel : PageModel
    {
        private ILogger<JoinSessionModel> _logger;
        private readonly ISessionRepository _repository;

        public string SessionTitle { get; set; }
        
        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [RegularExpression("[\\w-]+")]
        public string ParticipantUserName { get; set; }

        public JoinSessionModel(ISessionRepository repository, ILogger<JoinSessionModel> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var session = await _repository.GetAsync(id);

            if (session == null)
                return NotFound();

            Id = session.Id;
            SessionTitle = session.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var session = await _repository.GetAsync(Id);
            try
            {
                session.AddParticipant(ParticipantUserName);
            }
            catch (SessionExceptions e)
            {
                ModelState.AddModelError(nameof(ParticipantUserName), e.Message);
                return Page();
            }

            await _repository.UnitOfWork.SaveEntitiesAsync();

            _logger.LogDebug($"Participant {ParticipantUserName}` is joining session id {Id}");

            return Redirect($"/App/PollSession/{Id}/Username/{ParticipantUserName}");
        }
    }
}
