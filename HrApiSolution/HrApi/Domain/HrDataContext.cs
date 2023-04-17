using Microsoft.EntityFrameworkCore;

namespace HrApi.Domain;

public class HrDataContext : DbContext
{
    public HrDataContext(DbContextOptions<HrDataContext> options) : base(options) { }

    // Give all the entity classes it should track in the database
    public DbSet<DepartmentEntity> Departments { get; set; }
}
