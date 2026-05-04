using StudentsBlazorApp.Components.Pages.Dashboard;

namespace StudentsBlazorApp.Services
{
    public interface IDashboardService
    {
        Task<DashboardSnapshot> GetDashboardSnapshotAsync(CancellationToken cancellationToken = default);
        Task AddStudentAsync(StudentUpsertRequest request, CancellationToken cancellationToken = default);
        Task UpdateStudentAsync(StudentUpdateRequest request, CancellationToken cancellationToken = default);
        Task DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddCourseAsync(CourseCreateRequest request, CancellationToken cancellationToken = default);
        Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAssignmentAsync(AssignmentCreateRequest request, CancellationToken cancellationToken = default);
        Task DeleteAssignmentAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddEnrollmentAsync(EnrollmentCreateRequest request, CancellationToken cancellationToken = default);
        Task DeleteEnrollmentAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddGradeAsync(GradeCreateRequest request, CancellationToken cancellationToken = default);
    }
}
