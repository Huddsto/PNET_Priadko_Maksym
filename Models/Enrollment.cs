namespace StudentsBlazorApp.Models
{
    public class Enrollment
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Student? Student { get; set; }

        public Guid CourseId { get; set; }
        public Course? Course { get; set; }

        public DateTime EnrolledAt { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
