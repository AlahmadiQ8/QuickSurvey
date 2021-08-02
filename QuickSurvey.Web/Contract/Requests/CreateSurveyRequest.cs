using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Web.Controllers.Requests
{
    public class CreateSurveyRequest : IValidatableObject
    {
        [Required]
        [MinLength(5)]
        [MaxLength(250)]
        public string Title { get; set; }
        
        [Required]
        public IList<string> Choices { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Choices.Count > Session.MaxNumberOfChoices)
                yield return new ValidationResult($"Number of choices cannot exceed {Session.MaxNumberOfChoices}.");

            if (Choices.Count < 2)
                yield return new ValidationResult($"Choices must contain at least two choices");

            if (Choices.Any(c => c.Length < 5 || c.Length > 250))
                yield return new ValidationResult("Each choice must have character count between 5 and 250");
        }
    }
}
