using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public DashboardModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public string? Username { get; set; }

        public int PendingCount { get; set; }

        public int InProgressCount { get; set; }

        public int CompletedCount { get; set; }

        public int DeferredCount { get; set; }

        public int CancelledCount { get; set; }

        public IActionResult OnGet(string? username)
        {
            string? loggedInUser =
                HttpContext.Session.GetString("LoggedInUser");

            if (string.IsNullOrEmpty(loggedInUser))
            {
                return RedirectToPage("/Login");
            }

            Username = loggedInUser;

            LoadMDCounts();

            return Page();
        }

        private void LoadMDCounts()
        {
            var records = _db.MDRecords
                .AsNoTracking()
                .ToList();

            PendingCount = records.Count(x =>
                string.Equals(
                    x.Status,
                    "Not Started",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    x.Status,
                    "Deferred",
                    StringComparison.OrdinalIgnoreCase));

            InProgressCount = records.Count(x =>
                string.Equals(
                    x.Status,
                    "In Progress",
                    StringComparison.OrdinalIgnoreCase));

            CompletedCount = records.Count(x =>
                string.Equals(
                    x.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase));

            DeferredCount = records.Count(x =>
                string.Equals(
                    x.Status,
                    "Deferred",
                    StringComparison.OrdinalIgnoreCase));

            CancelledCount = records.Count(x =>
                string.Equals(
                    x.Status,
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase));
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}