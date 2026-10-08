using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class GlobalSearchModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public GlobalSearchModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public List<MDEntity> Results { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                return;
            }

            Query = Query.Trim();

            Results = await _db.MDRecords
                .AsNoTracking()
                .Where(x =>
                    (x.MilestoneName != null &&
                     x.MilestoneName.Contains(Query)) ||

                    (x.DeliverableName != null &&
                     x.DeliverableName.Contains(Query)) ||

                    (x.AssignedTo != null &&
                     x.AssignedTo.Contains(Query)) ||

                    (x.Status != null &&
                     x.Status.Contains(Query)))
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
    }
}