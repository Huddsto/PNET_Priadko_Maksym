using StudentsBlazorApp.Models;

namespace StudentsBlazorApp.Services
{
    public sealed partial class DashboardService
    {
        private async Task WriteAuditLogAsync(
            string entityName,
            string actionName,
            Guid entityId,
            string? details,
            CancellationToken cancellationToken)
        {
            db.AuditLogs.Add(new AuditLog
            {
                CreatedAt = DateTime.UtcNow,
                EntityName = entityName,
                ActionName = actionName,
                EntityId = entityId.ToString(),
                Details = details
            });

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
