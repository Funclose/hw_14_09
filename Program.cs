using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Cryptography;
using System.Xml;

namespace HW_14_09
{
     class Program
    {
        static void Main(string[] args)
        {
            //using (ApplicationContext context = new ApplicationContext())
            //{
            //    context.Database.EnsureDeleted();
            //    context.Database.EnsureCreated();

            //}
            //var students = new List<Student>
            //{
            //    new Student { FirstName = "John", LastName = "Doe", Birthday = new DateTime(2000, 4, 15) },
            //    new Student { FirstName = "Jane", LastName = "Smith", Birthday = new DateTime(1999, 8, 23) },
            //    new Student { FirstName = "Michael", LastName = "Brown", Birthday = new DateTime(2001, 12, 2) },
            //    new Student { FirstName = "Emily", LastName = "Johnson", Birthday = new DateTime(2000, 3, 10) },
            //    new Student { FirstName = "David", LastName = "Williams", Birthday = new DateTime(1998, 11, 30) }
            //};
            //var courses = new List<Course>
            //{
            //    new Course { Name = "Mathematics", Description = "Basic principles of mathematics." },
            //    new Course { Name = "Computer Science", Description = "Introduction to computer science concepts." },
            //    new Course { Name = "History", Description = "World history overview." },
            //    new Course { Name = "Physics", Description = "Fundamentals of physics and its laws." },
            //    new Course { Name = "Chemistry", Description = "Introduction to chemical reactions and elements." }
            //};
            //var instructors = new List<Instructor>
            //{
            //    new Instructor { FirstName = "Alice", LastName = "Davis" },
            //    new Instructor { FirstName = "Bob", LastName = "Miller" },
            //    new Instructor { FirstName = "Charlie", LastName = "Wilson" },
            //    new Instructor { FirstName = "Diana", LastName = "Moore" },
            //    new Instructor { FirstName = "Edward", LastName = "Taylor" }
            //};
            //var enrollments = new List<Enrollment>
            //{
            //    new Enrollment { Student = students[0], Course = courses[0], EnrollmentDate = DateTime.Now },
            //    new Enrollment { Student = students[1], Course = courses[1], EnrollmentDate = DateTime.Now },
            //    new Enrollment { Student = students[2], Course = courses[2], EnrollmentDate = DateTime.Now },
            //    new Enrollment { Student = students[3], Course = courses[3], EnrollmentDate = DateTime.Now },
            //    new Enrollment { Student = students[4], Course = courses[4], EnrollmentDate = DateTime.Now }
            //};
            //using (var context = new ApplicationContext())
            //{
            //    context.Students.AddRange(students);
            //    context.Courses.AddRange(courses);
            //    context.Instructors.AddRange(instructors);
            //    context.Enrollments.AddRange(enrollments);
            //    context.SaveChanges();
            //}
            using (ApplicationContext db = new ApplicationContext())
            {

                //1
                var StudentEnroll = db.Enrollments
                    .Where(e => e.CourseId ==1).Select(e => e.Student).ToList();

                //2
                var CoursesTeachInstructor = db.Courses.
                    Where(e => e.Instructor.Any(i => i.Id == 1)).ToList();

                //3

                var CoursesWithStudent = db.Courses
                    .Where(e => e.Instructor.Any(i => i.Id ==1))
                    .Select(c => new { 
                        Course =c,
                        Student = c.Enrollments.Select(e => e.Student).ToList()
                    }).ToList(); 

                //4
                var CourseWithMore5Student = db.Courses
                    .Where(e => e.Enrollments.Count() > 5)
                    .ToList();

                //5
                var StudentOlder25 = db.Students
                    .Where(e => DateTime.Now.Year - e.Birthday.Year > 25);

                //6
                var Average = db.Students
                    .Average(e => DateTime.Now.Year - e.Birthday.Year);
                //7
                var younger = db.Students
                    .OrderBy(e => e.Birthday).FirstOrDefault();
            }
        }
    }
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public DateTime Birthday { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Instructor> Instructor { get; set; }
    }

    public class Enrollment
    { 
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public  DateTime EnrollmentDate { get; set; }
    }

    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Course> Courses { get; set; } = new();
    }

    

    public class ApplicationContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });

            optionsBuilder.UseSqlServer("Server=DESKTOP-TBASQVJ;Database=Univercity;Trusted_Connection=True;TrustServerCertificate=True;");

            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructor)
                .WithMany(i => i.Courses)
                .UsingEntity(j => j.ToTable("CoursesInstructor"));
        }
    }
}
