using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

        await context.Database.EnsureCreatedAsync();
        await EnsureSchemaAsync(context);

        var ownerEmail = configuration["OwnerAdmin:Email"] ?? "owner@eschool.app";
        var ownerPassword = configuration["OwnerAdmin:Password"] ?? "Owner@123";
        var ownerName = configuration["OwnerAdmin:FullName"] ?? "Platform Owner";
        var platformTenant = await context.Tenants.FirstOrDefaultAsync(t => t.SchoolCode == "OWNR");

        if (platformTenant == null)
        {
            platformTenant = new Tenant
            {
                SchoolCode = "OWNR",
                SchoolName = "eSchool Platform",
                Email = ownerEmail,
                Phone = configuration["OwnerAdmin:Phone"] ?? "+10000000000",
                Address = "Platform",
                ContactPersonName = ownerName,
                IsActive = true,
                SubscriptionStart = DateTime.UtcNow,
                SubscriptionEnd = DateTime.UtcNow.AddYears(10)
            };

            context.Tenants.Add(platformTenant);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Owner))
        {
            context.Users.Add(new User
            {
                TenantId = platformTenant.Id,
                Email = ownerEmail,
                FullName = ownerName,
                Phone = configuration["OwnerAdmin:Phone"] ?? "+10000000000",
                Role = UserRole.Owner,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(ownerPassword)
            });

            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureSchemaAsync(SchoolDbContext context)
    {
        await EnsureColumnAsync(context, "Tenants", "ContactPersonName", "TEXT NOT NULL DEFAULT ''");
        await EnsureColumnAsync(context, "Students", "ParentUserId", "TEXT NULL");
        await EnsureColumnAsync(context, "Students", "ClassId", "TEXT NULL");
        await EnsureColumnAsync(context, "Faculties", "Username", "TEXT NULL");

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "Classes" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Classes" PRIMARY KEY,
                "TenantId" TEXT NOT NULL,
                "Name" TEXT NOT NULL,
                "Section" TEXT NOT NULL,
                "ClassTeacherUserId" TEXT NULL,
                "Capacity" INTEGER NOT NULL DEFAULT 0
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Classes_TenantId_Name_Section"
            ON "Classes" ("TenantId", "Name", "Section");
            """);
    }

    private static async Task EnsureColumnAsync(SchoolDbContext context, string tableName, string columnName, string columnDefinition)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{tableName}\")";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                return;
        }

        var alterSql = $"ALTER TABLE \"{tableName}\" ADD COLUMN \"{columnName}\" {columnDefinition};";
        await context.Database.ExecuteSqlRawAsync(alterSql);
    }
}
