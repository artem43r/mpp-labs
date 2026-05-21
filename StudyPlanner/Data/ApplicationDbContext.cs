using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Models;

namespace StudyPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentTag> AssignmentTags { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальность Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Уникальность UserName
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // Уникальность тегов
            modelBuilder.Entity<Tag>()
                .HasIndex(t => new { t.Name, t.OwnerId })
                .IsUnique();

            // Subject → User
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

            // Assignment → User
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment → Subject
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Subject)
                .WithMany(s => s.Assignments)
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // AssignmentTag
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

            // Comment → Assignment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Assignment)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}