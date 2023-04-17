using Microsoft.EntityFrameworkCore;

namespace HrApi.Domain;

public class HrDataContext : DbContext
{
    public HrDataContext(DbContextOptions<HrDataContext> options) : base(options) { }

    // Give all the entity classes it should track in the database
    public DbSet<DepartmentEntity> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DepartmentEntity>().Property(p => p.Name).HasMaxLength(20);
        modelBuilder.Entity<DepartmentEntity>().HasIndex(p => p.Name).IsUnique();
    }
}
