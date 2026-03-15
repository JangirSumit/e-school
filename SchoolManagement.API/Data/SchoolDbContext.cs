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
    public DbSet<SchoolClass> Classes { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Mark> Marks { get; set; }
    public DbSet<Holiday> Holidays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>().HasKey(t => t.Id);
        modelBuilder.Entity<Tenant>().HasIndex(t => t.SchoolCode).IsUnique();
        
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().HasIndex(u => new { u.TenantId, u.Username }).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email);
        modelBuilder.Entity<Student>().HasKey(s => s.Id);
        modelBuilder.Entity<Faculty>().HasKey(f => f.Id);
        modelBuilder.Entity<SchoolClass>().HasKey(c => c.Id);
        modelBuilder.Entity<SchoolClass>().HasIndex(c => new { c.TenantId, c.Name, c.Section }).IsUnique();
        modelBuilder.Entity<Attendance>().HasKey(a => a.Id);
        modelBuilder.Entity<Mark>().HasKey(m => m.Id);
        modelBuilder.Entity<Holiday>().HasKey(h => h.Id);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Tenant)
            .WithMany(t => t.Students)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.ParentUser)
            .WithMany()
            .HasForeignKey(s => s.ParentUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.SchoolClass)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Faculty>()
            .HasOne(f => f.Tenant)
            .WithMany(t => t.Faculties)
            .HasForeignKey(f => f.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Faculty>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SchoolClass>()
            .HasOne(c => c.Tenant)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SchoolClass>()
            .HasOne(c => c.ClassTeacherUser)
            .WithMany()
            .HasForeignKey(c => c.ClassTeacherUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
