using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class DashboardModel : PageModel
    {
        public string? Username { get; set; }

        public IActionResult OnGet(string? username)
        {
            string? loggedInUser =
                HttpContext.Session.GetString("LoggedInUser");

            if (string.IsNullOrEmpty(loggedInUser))
            {
                return RedirectToPage("/Login");
            }

            Username = loggedInUser;

            return Page();
        }
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}

