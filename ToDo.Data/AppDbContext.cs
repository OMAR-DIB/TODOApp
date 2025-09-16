using Microsoft.EntityFrameworkCore;
using ToDo.Data.DataSeed;
using ToDo.Data.Entities;

namespace ToDo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<ToDos> ToDos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region ToDos
            modelBuilder.Entity<ToDos>()
                .Property(p => p.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<ToDos>()
            .Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

            modelBuilder.Entity<ToDos>()
            .Property(x => x.ToDoAt)
            .IsRequired();
            #endregion
            #region User
            modelBuilder.Entity<User>().Property(x => x.Username)
                                       .IsRequired()
                                       .HasMaxLength(50);

            modelBuilder.Entity<User>().Property(x => x.Email)
                                        .IsRequired();

            modelBuilder.Entity<User>().Property(x => x.Password)
                                        .IsRequired();
            #endregion

            #region UserRole
            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
            #endregion

            #region SubTask
            modelBuilder.Entity<SubTask>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<SubTask>()
                .Property(x => x.Status)
                .HasConversion<string>()  
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<SubTask>()
                .HasOne(x => x.ToDo)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(x => x.ToDoId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region Notification
            modelBuilder.Entity<Notification>()
                .Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                 .OnDelete(DeleteBehavior.Restrict);  // no cascade here

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ToDo)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.ToDoId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion


            modelBuilder.SeedData();
        }
    }
}