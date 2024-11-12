using Microsoft.EntityFrameworkCore;
using WizardSoftTestTaskAPI.Models;

namespace WizardSoftTestTaskAPI.Data
{
    /// <summary>
    /// Настройка базы данных Catalogs
    /// </summary>
    public class CatalogsDbContext : DbContext
    {
        public CatalogsDbContext(DbContextOptions<CatalogsDbContext> options) : base(options) { }

        public DbSet<Catalog> Catalogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Catalog>()
                        .HasMany(c => c.Children)
                        .WithOne(c => c.Parent)
                        .HasForeignKey(c => c.ParentId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}