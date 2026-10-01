using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Pages
{
    public class GuidelinesModel : PageModel
    {
        private readonly QMSPortalDbContext _db;

        public GuidelinesModel(QMSPortalDbContext db)
        {
            _db = db;
        }

        public List<Guideline> Guidelines { get; set; } = new();

        public async Task OnGetAsync()
        {
            Guidelines = await _db.Guidelines
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }
    }
}