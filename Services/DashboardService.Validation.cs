using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StudentsBlazorApp.Models;

namespace StudentsBlazorApp.Services
{
    public sealed partial class DashboardService
    {
        private static void ValidateStudent(Student student)
        {
            ValidateDataAnnotations(student);

            if (student.Age is < 1 or > 150)
            {
                throw new ValidationException("Student age must be between 1 and 150.");
            }
        }

        private static void ValidateTeacher(Teacher teacher)
        {
            ValidateRequiredText(teacher.Name, "Teacher name");

            if (teacher.Name.Length > 100)
            {
                throw new ValidationException("Teacher name must be 100 characters or fewer.");
            }

            if (teacher.Department?.Length > 100)
            {
                throw new ValidationException("Department must be 100 characters or fewer.");
            }
        }

        private static void ValidateCourse(Course course)
        {
            ValidateRequiredText(course.Title, "Course title");

            if (course.Title.Length > 200)
            {
                throw new ValidationException("Course title must be 200 characters or fewer.");
            }
        }

        private static void ValidateAssignment(Assignment assignment)
        {
            ValidateRequiredText(assignment.Title, "Assignment title");

            if (assignment.Title.Length > 200)
            {
                throw new ValidationException("Assignment title must be 200 characters or fewer.");
            }
        }

        private static void ValidateEnrollment(Enrollment enrollment)
        {
            if (enrollment.StudentId == Guid.Empty)
            {
                throw new ValidationException("Student is required.");
            }

            if (enrollment.CourseId == Guid.Empty)
            {
                throw new ValidationException("Course is required.");
            }
        }

        private static void ValidateGrade(Grade grade)
        {
            if (grade.EnrollmentId == Guid.Empty)
            {
                throw new ValidationException("Enrollment is required.");
            }

            if (grade.AssignmentId == Guid.Empty)
            {
                throw new ValidationException("Assignment is required.");
            }

            if (grade.Value is < 0 or > 100)
            {
                throw new ValidationException("Grade must be between 0 and 100.");
            }
        }

        private static void ValidateRequiredText(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException($"{fieldName} is required.");
            }
        }

        private static void ValidateDataAnnotations(object instance)
        {
            var context = new ValidationContext(instance);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(instance, context, results, validateAllProperties: true))
            {
                throw new ValidationException(results.First().ErrorMessage);
            }
        }

        private async Task EnsureStudentEmailIsUniqueAsync(string email, Guid? currentStudentId, CancellationToken cancellationToken)
        {
            var exists = await db.Students
                .AsNoTracking()
                .AnyAsync(
                    s => s.Email == email && (!currentStudentId.HasValue || s.Id != currentStudentId.Value),
                    cancellationToken);

            if (exists)
            {
                throw new ValidationException("A student with this email already exists.");
            }
        }

        private async Task EnsureStudentExistsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var exists = await db.Students
                .AsNoTracking()
                .AnyAsync(s => s.Id == studentId, cancellationToken);

            if (!exists)
            {
                throw new ValidationException("Student not found.");
            }
        }

        private async Task EnsureCourseExistsAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var exists = await db.Courses
                .AsNoTracking()
                .AnyAsync(c => c.Id == courseId, cancellationToken);

            if (!exists)
            {
                throw new ValidationException("Course not found.");
            }
        }

        private async Task EnsureEnrollmentIsUniqueAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
        {
            var exists = await db.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

            if (exists)
            {
                throw new ValidationException("This student is already enrolled in the selected course.");
            }
        }
    }
}
