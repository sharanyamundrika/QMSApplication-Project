using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMSApplication.Pages
{
    public class ComingSoonModel : PageModel
    {
        public string? Module { get; set; }

        public void OnGet(string? module)
        {
            Module = module;
        }
    }
}