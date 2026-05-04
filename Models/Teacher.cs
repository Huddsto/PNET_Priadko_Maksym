namespace StudentsBlazorApp.Models
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Department { get; set; }
    }
}
