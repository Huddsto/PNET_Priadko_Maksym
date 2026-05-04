namespace StudentsBlazorApp.Models
{
    public class Grade
    {
        public Guid Id { get; set; }

        public Guid EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public Guid AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        public int Value { get; set; }
    }
}
