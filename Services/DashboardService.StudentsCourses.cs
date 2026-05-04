using Microsoft.EntityFrameworkCore;
using StudentsBlazorApp.Components.Pages.Dashboard;
using StudentsBlazorApp.Models;

namespace StudentsBlazorApp.Services
{
    public sealed partial class DashboardService
    {
        public async Task AddStudentAsync(StudentUpsertRequest request, CancellationToken cancellationToken = default)
        {
            var student = new Student
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                Age = request.Age
            };

            ValidateStudent(student);
            await EnsureStudentEmailIsUniqueAsync(student.Email, null, cancellationToken);

            db.Students.Add(student);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Student", "Create", student.Id, $"Email={student.Email}; Age={student.Age}", cancellationToken);
            logger.LogInformation("Student created: {Email}", request.Email);
        }

        public async Task UpdateStudentAsync(StudentUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var student = await db.Students.FindAsync([request.Id], cancellationToken);
            if (student is null)
            {
                logger.LogWarning("Student update failed. Student {StudentId} not found.", request.Id);
                throw new InvalidOperationException("Student not found.");
            }

            student.Name = request.Name.Trim();
            student.Email = request.Email.Trim();
            student.Age = request.Age;

            ValidateStudent(student);
            await EnsureStudentEmailIsUniqueAsync(student.Email, student.Id, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Student", "Update", student.Id, $"Email={student.Email}; Age={student.Age}", cancellationToken);
            logger.LogInformation("Student updated: {StudentId}", request.Id);
        }

        public async Task DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var studentEnrollmentIds = await db.Enrollments
                .Where(e => e.StudentId == id)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            var relatedGrades = await db.Grades
                .Where(g => studentEnrollmentIds.Contains(g.EnrollmentId))
                .ToListAsync(cancellationToken);

            var relatedEnrollments = await db.Enrollments
                .Where(e => e.StudentId == id)
                .ToListAsync(cancellationToken);

            var student = await db.Students.FindAsync([id], cancellationToken);
            if (student is null)
            {
                logger.LogWarning("Student delete skipped. Student {StudentId} not found.", id);
                return;
            }

            if (relatedGrades.Count > 0)
            {
                db.Grades.RemoveRange(relatedGrades);
            }

            if (relatedEnrollments.Count > 0)
            {
                db.Enrollments.RemoveRange(relatedEnrollments);
            }

            db.Students.Remove(student);
            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Student", "Delete", id, $"Removed student {student.Email}", cancellationToken);
            logger.LogInformation("Student deleted: {StudentId}", id);
        }

        public async Task AddCourseAsync(CourseCreateRequest request, CancellationToken cancellationToken = default)
        {
            var teacherName = request.TeacherName.Trim();
            var department = string.IsNullOrWhiteSpace(request.Department) ? null : request.Department.Trim();

            ValidateRequiredText(request.Title, "Course title");
            ValidateRequiredText(teacherName, "Teacher name");

            var teacher = await db.Teachers.FirstOrDefaultAsync(t =>
                t.Name == teacherName &&
                t.Department == department, cancellationToken);

            if (teacher is null)
            {
                teacher = new Teacher
                {
                    Name = teacherName,
                    Department = department
                };

                ValidateTeacher(teacher);
                db.Teachers.Add(teacher);
                await db.SaveChangesAsync(cancellationToken);
                await WriteAuditLogAsync("Teacher", "Create", teacher.Id, $"Name={teacher.Name}; Department={teacher.Department}", cancellationToken);
                logger.LogInformation("Teacher created implicitly for course creation: {TeacherName}", teacherName);
            }

            var course = new Course
            {
                Title = request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                TeacherId = teacher.Id
            };

            ValidateCourse(course);
            db.Courses.Add(course);

            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Course", "Create", course.Id, $"Title={course.Title}; TeacherId={course.TeacherId}", cancellationToken);
            logger.LogInformation("Course created: {CourseTitle}", request.Title);
        }

        public async Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var courseAssignmentIds = await db.Assignments
                .Where(a => a.CourseId == id)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            var courseEnrollmentIds = await db.Enrollments
                .Where(e => e.CourseId == id)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            var courseGrades = await db.Grades
                .Where(g => courseAssignmentIds.Contains(g.AssignmentId) || courseEnrollmentIds.Contains(g.EnrollmentId))
                .ToListAsync(cancellationToken);

            var courseAssignments = await db.Assignments
                .Where(a => a.CourseId == id)
                .ToListAsync(cancellationToken);

            var courseEnrollments = await db.Enrollments
                .Where(e => e.CourseId == id)
                .ToListAsync(cancellationToken);

            var course = await db.Courses.FindAsync([id], cancellationToken);
            if (course is null)
            {
                logger.LogWarning("Course delete skipped. Course {CourseId} not found.", id);
                return;
            }

            if (courseGrades.Count > 0)
            {
                db.Grades.RemoveRange(courseGrades.DistinctBy(g => g.Id));
            }

            if (courseAssignments.Count > 0)
            {
                db.Assignments.RemoveRange(courseAssignments);
            }

            if (courseEnrollments.Count > 0)
            {
                db.Enrollments.RemoveRange(courseEnrollments);
            }

            db.Courses.Remove(course);
            await db.SaveChangesAsync(cancellationToken);
            await WriteAuditLogAsync("Course", "Delete", id, $"Removed course {course.Title}", cancellationToken);
            logger.LogInformation("Course deleted: {CourseId}", id);
        }
    }
}
