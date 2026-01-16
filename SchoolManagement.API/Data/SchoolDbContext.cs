using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Mark> Marks { get; set; }
    public DbSet<Holiday> Holidays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>().HasKey(t => t.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Student>().HasKey(s => s.Id);
        modelBuilder.Entity<Faculty>().HasKey(f => f.Id);
        modelBuilder.Entity<Attendance>().HasKey(a => a.Id);
        modelBuilder.Entity<Mark>().HasKey(m => m.Id);
        modelBuilder.Entity<Holiday>().HasKey(h => h.Id);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Tenant)
            .WithMany(t => t.Students)
            .HasForeignKey(s => s.TenantId);

        modelBuilder.Entity<Faculty>()
            .HasOne(f => f.Tenant)
            .WithMany(t => t.Faculties)
            .HasForeignKey(f => f.TenantId);
    }
}
