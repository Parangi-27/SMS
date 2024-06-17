using Microsoft.EntityFrameworkCore;
using Repository.Model;

namespace Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().HasData(
              new Subject
              {
                  Id = 1,
                  SubjectName = "Maths",
              },
              new Subject
              {
                  Id = 2,
                  SubjectName = "English",
              },
              new Subject
              {
                  Id = 3,
                  SubjectName = "Science",
              },
              new Subject
              {
                  Id = 4,
                  SubjectName = "Social Science",
              }
              );

            modelBuilder.Entity<Users>().HasData(
              new Users
              {
                  Id = 1,
                  Name = "Admin",
                  Email = "admin@gmail.com",
                  Password = "Password",
                  DateOfBirth = new DateTime(1990, 1, 1),
                  DateOfEnrollment = new DateTime(1990, 1, 1),
                  Role = RoleTypes.Admin,
                  IsActive = true,
                  IsPasswordReset = true,
              }
              );

            modelBuilder.Entity<Attendance>()
            .HasIndex(a => new { a.TeacherId, a.Date })
            .HasDatabaseName("IX_Teacher_Subject_Date");

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.Date })
                .HasDatabaseName("IX_Student_Date");

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.ClassLevel, a.Date })
                .HasDatabaseName("IX_Class_Date");

            modelBuilder.Entity<Grades>()
               .HasIndex(g => g.ExamMonth)
               .HasDatabaseName("IX_Grades_ExamMonth");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }


    }
}
