using StudentsBlazorApp.Models;

namespace StudentsBlazorApp.Services
{
    public sealed class DashboardSnapshot
    {
        public required List<Student> Students { get; init; }
        public required List<Teacher> Teachers { get; init; }
        public required List<Course> Courses { get; init; }
        public required List<Assignment> Assignments { get; init; }
        public required List<Enrollment> Enrollments { get; init; }
        public required List<Grade> Grades { get; init; }
    }
}
