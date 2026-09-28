using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class WelcomeModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }

        public void OnGet(string? username)
        {
            Username = username;
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(
                "/Dashboard",
                new { username = Username }
            );
        }
             public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}
        
    
