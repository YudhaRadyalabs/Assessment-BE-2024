using Microsoft.EntityFrameworkCore;
using persistences.Extentions;
using persistences.Entities;
using infrastructures.Services;

namespace api_ticket.EntityFrameworks.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entitiesAssembly = typeof(Program).Assembly;
            modelBuilder.RegisterAllEntities<BaseEntity>(entitiesAssembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Guid? performer = null;
            string performerName = "SYSTEM";

            if (!string.IsNullOrEmpty(_currentUserService.Username))
                performerName = _currentUserService.Username;

            if (_currentUserService.Id != Guid.Empty)
                performer = _currentUserService.Id;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = performer;
                        entry.Entity.CreatedByName = performerName;
                        entry.Entity.CreatedDate = DateTime.UtcNow.AddHours(7);
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = performer;
                        entry.Entity.UpdatedByName = performerName;
                        entry.Entity.UpdatedDate = DateTime.UtcNow.AddHours(7);
                        break;
                    case EntityState.Deleted:
                        entry.Entity.DeletedBy = performer;
                        entry.Entity.DeletedByName = performerName;
                        entry.Entity.DeletedDate = DateTime.UtcNow.AddHours(7);
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }
            /*** Add Audit Trail ***/
            //new AuditHelper(this).AddAuditLogs(performer);
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
