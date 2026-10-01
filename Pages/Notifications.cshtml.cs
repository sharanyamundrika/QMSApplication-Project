using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public NotificationsModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public List<Notification> Notifications { get; set; } = new();

        public async Task OnGetAsync()
        {
            Notifications = await _db.Notifications
                .AsNoTracking()
                .Where(x => x.IsPublished)
                .OrderByDescending(x => x.NotificationDateUtc)
                .ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync(long id)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            notification.IsRead = true;
            notification.IsAcknowledged = true;
            notification.ReadAtUtc = now;
            notification.AcknowledgedAtUtc = now;
            notification.UpdatedAtUtc = now;

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}