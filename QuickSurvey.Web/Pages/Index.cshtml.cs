using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuickSurvey.Web.Pages
{
    public class IndexModel : PageModel
    {
        public string WelcomeMessage = "Welcome to this Page";
        public void OnGet()
        {
        }
    }
}
