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

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            if (!IsAdminOrManager())
            {
                return RedirectToPage("/Notifications");
            }

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            _db.Notifications.Remove(notification);

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostTogglePublishAsync(long id)
        {
            if (!IsAdminOrManager())
            {
                return RedirectToPage("/Notifications");
            }

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.IsPublished = !notification.IsPublished;
            notification.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}