using Microsoft.EntityFrameworkCore;
using StudyPlanner.Models;

namespace StudyPlanner.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet для каждой сущности
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }      // Project → Subject
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Assignment> Assignments { get; set; } // Task → Assignment
        public DbSet<AssignmentTag> AssignmentTags { get; set; } // TaskTag → AssignmentTag
        public DbSet<Comment> Comments { get; set; } // ДОП

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальность Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Уникальность Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Уникальность Tag (в рамках пользователя)
            modelBuilder.Entity<Tag>()
                .HasIndex(t => new { t.Name, t.OwnerId })
                .IsUnique();

            // Subject → User (Project → User)
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Owner)
                .WithMany(u => u.Subjects)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tag → User
            modelBuilder.Entity<Tag>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.Tags)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment → User (Task → User)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment → Subject (Task → Project)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Subject)
                .WithMany(s => s.Assignments)
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // AssignmentTag (M:N)
            modelBuilder.Entity<AssignmentTag>()
                .HasOne(at => at.Assignment)
                .WithMany(a => a.AssignmentTags)
                .HasForeignKey(at => at.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignmentTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.AssignmentTags)
                .HasForeignKey(at => at.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → Assignment (ДОП)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Assignment)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → User (ДОП)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}