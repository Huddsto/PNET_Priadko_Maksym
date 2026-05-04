namespace StudentsBlazorApp.Models
{
    public class Course
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
