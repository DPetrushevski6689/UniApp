using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.Data
{
    public class UniAppContext : IdentityDbContext<AppUser>
    {
        public UniAppContext() { }

        public UniAppContext(DbContextOptions<UniAppContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Course>()
                .HasOne<Teacher>(m => m.FirstTeacher)
                .WithMany(m => m.FirstCourses)
                .HasForeignKey(m => m.FirstTeacherId);

            builder.Entity<Course>()
               .HasOne<Teacher>(m => m.SecondTeacher)
               .WithMany(m => m.SecondCourses)
               .HasForeignKey(m => m.SecondTeacherId);

            builder.Entity<Enrollment>()
                .HasOne<Student>(m => m.Student)
                .WithMany(m => m.Courses)
                .HasForeignKey(m => m.StudentId);

            builder.Entity<Enrollment>()
                .HasOne<Course>(m => m.Course)
                .WithMany(m => m.Students)
                .HasForeignKey(m => m.CourseId);
        }

        public DbSet<UniApp.Models.Student> Student { get; set; }

        public DbSet<UniApp.Models.Course> Course { get; set; }

        public DbSet<UniApp.Models.Teacher> Teacher { get; set; }

        public DbSet<UniApp.Models.Enrollment> Enrollment { get; set; }
    }
}
