using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationAsp.Entities;

namespace WebApplicationAsp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Assessment> Assessments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Assessment>()
                .HasIndex(a => new { a.ApplicationId, a.ItemId })
                .IsUnique();

            builder.Entity<Application>()
                .HasOne(a => a.Owner)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Assessment>()
                .HasOne(a => a.AssessedBy)
                .WithMany()
                .HasForeignKey(a => a.AssessedById)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
