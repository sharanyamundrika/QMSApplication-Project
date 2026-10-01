using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class NotificationAdminModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public NotificationAdminModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public List<Notification> Notifications { get; set; } = new();

        private bool IsAdminOrManager()
        {
            var role = HttpContext.Session.GetString("UserRole");

            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminOrManager())
            {
                return RedirectToPage("/Notifications");
            }

            Notifications = await _db.Notifications
                .AsNoTracking()
                .OrderByDescending(x => x.NotificationDateUtc)
                .ToListAsync();

            return Page();
        }
    }
}