namespace StudentsBlazorApp.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? Details { get; set; }
    }
}
