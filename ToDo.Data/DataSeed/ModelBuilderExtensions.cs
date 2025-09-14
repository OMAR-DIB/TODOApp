using Microsoft.EntityFrameworkCore;
using ToDo.Data.Entities;
using ToDo.Data.Entities.Base;
namespace ToDo.Data.DataSeed
{
    public static class ModelBuilderExtensions
    {
        private static DateTime timestamp = new DateTime(2025, 9, 6, 0, 0, 0, 0);

        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.SeedRoles();
        }

        private static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp,
                    IsDeleted = false,
                    CreatedBy = "System"
                },
                new Role
                {
                    Id = 2,
                    Name = "User",
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp,
                    IsDeleted = false,
                    CreatedBy = "System"
                }
            );
        }
    }
}
