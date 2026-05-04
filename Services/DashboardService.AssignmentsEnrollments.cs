using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StudentsBlazorApp.Components.Pages.Dashboard;
using StudentsBlazorApp.Models;

namespace StudentsBlazorApp.Services
{
    public sealed partial class DashboardService
    {
        public async Task AddAssignmentAsync(AssignmentCreateRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureCourseExistsAsync(request.CourseId, cancellationToken);

            var assignment = new Assignment
            {
                CourseId = request.CourseId,
                Title = request.Title.Trim(),
                DueDate = request.DueDate?.Date
            };

            ValidateAssignment(assignment);
            db.Assignments.Add(assignment);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Assignment", "Create", assignment.Id, $"CourseId={assignment.CourseId}; Title={assignment.Title}", cancellationToken);
            logger.LogInformation("Assignment created for course {CourseId}: {Title}", request.CourseId, request.Title);
        }

        public async Task DeleteAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var assignmentGrades = await db.Grades
                .Where(g => g.AssignmentId == id)
                .ToListAsync(cancellationToken);

            var assignment = await db.Assignments.FindAsync([id], cancellationToken);
            if (assignment is null)
            {
                logger.LogWarning("Assignment delete skipped. Assignment {AssignmentId} not found.", id);
                return;
            }

            if (assignmentGrades.Count > 0)
            {
                db.Grades.RemoveRange(assignmentGrades);
            }

            db.Assignments.Remove(assignment);
            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Assignment", "Delete", id, $"Removed assignment {assignment.Title}", cancellationToken);
            logger.LogInformation("Assignment deleted: {AssignmentId}", id);
        }

        public async Task AddEnrollmentAsync(EnrollmentCreateRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureStudentExistsAsync(request.StudentId, cancellationToken);
            await EnsureCourseExistsAsync(request.CourseId, cancellationToken);
            await EnsureEnrollmentIsUniqueAsync(request.StudentId, request.CourseId, cancellationToken);

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow
            };

            ValidateEnrollment(enrollment);
            db.Enrollments.Add(enrollment);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Enrollment", "Create", enrollment.Id, $"StudentId={enrollment.StudentId}; CourseId={enrollment.CourseId}", cancellationToken);
            logger.LogInformation("Enrollment created. Student: {StudentId}, Course: {CourseId}", request.StudentId, request.CourseId);
        }

        public async Task DeleteEnrollmentAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var enrollmentGrades = await db.Grades
                .Where(g => g.EnrollmentId == id)
                .ToListAsync(cancellationToken);

            var enrollment = await db.Enrollments.FindAsync([id], cancellationToken);
            if (enrollment is null)
            {
                logger.LogWarning("Enrollment delete skipped. Enrollment {EnrollmentId} not found.", id);
                return;
            }

            if (enrollmentGrades.Count > 0)
            {
                db.Grades.RemoveRange(enrollmentGrades);
            }

            db.Enrollments.Remove(enrollment);
            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Enrollment", "Delete", id, $"Removed enrollment for StudentId={enrollment.StudentId}; CourseId={enrollment.CourseId}", cancellationToken);
            logger.LogInformation("Enrollment deleted: {EnrollmentId}", id);
        }

        public async Task AddGradeAsync(GradeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var enrollment = await db.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment is null)
            {
                throw new ValidationException("Enrollment not found.");
            }

            var assignment = await db.Assignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, cancellationToken);

            if (assignment is null)
            {
                throw new ValidationException("Assignment not found.");
            }

            if (assignment.CourseId != enrollment.CourseId)
            {
                throw new ValidationException("Assignment does not belong to the selected enrollment course.");
            }

            var grade = new Grade
            {
                EnrollmentId = request.EnrollmentId,
                AssignmentId = request.AssignmentId,
                Value = request.Value
            };

            ValidateGrade(grade);
            db.Grades.Add(grade);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Grade", "Create", grade.Id, $"EnrollmentId={grade.EnrollmentId}; AssignmentId={grade.AssignmentId}; Value={grade.Value}", cancellationToken);
            logger.LogInformation(
                "Grade created. Enrollment: {EnrollmentId}, Assignment: {AssignmentId}, Grade: {Grade}",
                request.EnrollmentId,
                request.AssignmentId,
                request.Value);
        }
    }
}
