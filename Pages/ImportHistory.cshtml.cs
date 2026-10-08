using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class ImportHistoryModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public ImportHistoryModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public List<ImportBatch> ImportBatches { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var username =
                HttpContext.Session.GetString("Username");

            if (string.IsNullOrWhiteSpace(username))
            {
                return RedirectToPage("/Login");
            }

            ImportBatches = await _db.ImportBatches
                .AsNoTracking()
                .OrderByDescending(x => x.ImportedAtUtc)
                .ToListAsync();

            return Page();
        }
    }
}