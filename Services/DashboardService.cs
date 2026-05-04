using Microsoft.EntityFrameworkCore;
using StudentsBlazorApp.Data;

namespace StudentsBlazorApp.Services
{
    public sealed partial class DashboardService : IDashboardService
    {
        private readonly AppDbContext db;
        private readonly ILogger<DashboardService> logger;

        public DashboardService(AppDbContext db, ILogger<DashboardService> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task<DashboardSnapshot> GetDashboardSnapshotAsync(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Loading dashboard snapshot from database.");

            return new DashboardSnapshot
            {
                Students = await db.Students
                    .AsNoTracking()
                    .OrderBy(s => s.Name)
                    .ToListAsync(cancellationToken),

                Teachers = await db.Teachers
                    .AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToListAsync(cancellationToken),

                Courses = await db.Courses
                    .AsNoTracking()
                    .Include(c => c.Teacher)
                    .OrderBy(c => c.Title)
                    .ToListAsync(cancellationToken),

                Assignments = await db.Assignments
                    .AsNoTracking()
                    .Include(a => a.Course)
                        .ThenInclude(c => c!.Teacher)
                    .OrderBy(a => a.Title)
                    .ToListAsync(cancellationToken),

                Enrollments = await db.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                        .ThenInclude(c => c!.Teacher)
                    .OrderByDescending(e => e.EnrolledAt)
                    .ToListAsync(cancellationToken),

                Grades = await db.Grades
                    .AsNoTracking()
                    .Include(g => g.Assignment)
                    .OrderBy(g => g.Assignment!.Title)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}
