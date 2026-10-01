using Microsoft.EntityFrameworkCore;

namespace QMSApplication.Models
{
    public class QMSPortalDbContext : DbContext
    {
        public QMSPortalDbContext(
            DbContextOptions<QMSPortalDbContext> options)
            : base(options)
        {
        }
        public DbSet<MDEntity> MDRecords { get; set; }
        public DbSet<Guideline> Guidelines { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}