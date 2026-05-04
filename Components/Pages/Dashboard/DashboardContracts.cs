namespace StudentsBlazorApp.Components.Pages.Dashboard
{
    public sealed record DashboardStatRow(string Name, double Average);

    public sealed record StudentUpsertRequest(string Name, string Email, int? Age);

    public sealed record StudentUpdateRequest(Guid Id, string Name, string Email, int? Age);

    public sealed record CourseCreateRequest(string Title, string? Description, string TeacherName, string? Department);

    public sealed record AssignmentCreateRequest(Guid CourseId, string Title, DateTime? DueDate);

    public sealed record EnrollmentCreateRequest(Guid StudentId, Guid CourseId);

    public sealed record GradeCreateRequest(Guid EnrollmentId, Guid AssignmentId, int Value);
}
