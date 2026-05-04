namespace StudentsBlazorApp.Models
{
    public class Assignment
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
        public Course? Course { get; set; }

        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
