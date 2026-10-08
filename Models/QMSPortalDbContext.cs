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
        public DbSet<MDAuditLog> MDAuditLogs { get; set; }
        public DbSet<Guideline> Guidelines { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUserStatus> NotificationUserStatuses { get; set; }
        public DbSet<PortalUser> PortalUsers { get; set; }
        public DbSet<ImportBatch> ImportBatches { get; set; }

        public DbSet<ImportException> ImportExceptions { get; set; }
        protected override void OnModelCreating(
    ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MDEntity>()
                .Property(x => x.ConcurrencyToken)
                .IsConcurrencyToken();
        }
    }
}