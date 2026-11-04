using CampusFlow.Contracts.Enrollments;
using CampusFlow.Domain.Models;
using CampusFlow.ResourceAccess.Sqlite;
using CampusFlow.Rules.Enrollment;
using Microsoft.EntityFrameworkCore;

namespace CampusFlow.Services.Enrollment;

public class EnrollmentService(CampusFlowDbContext db, TimeProvider timeProvider, IEnumerable<IEnrollmentRule> rules) : IEnrollmentService
{
    public async Task<EnrollmentResult> EnrollAsync(int courseId, int studentId)
    {
        var course = await db.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new { c.Capacity, EnrolledCount = c.Enrollments.Count })
            .FirstOrDefaultAsync();

        if (course is null)
        {
            return EnrollmentResult.Failure(EnrollmentErrorKind.NotFound, $"Course {courseId} does not exist.");
        }

        var student = await db.Students
            .Where(s => s.Id == studentId)
            .Select(s => new
            {
                s.FirstName,
                s.LastName,
                EnrollmentCount = s.Enrollments.Count,
                AlreadyEnrolled = s.Enrollments.Any(e => e.CourseId == courseId)
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            return EnrollmentResult.Failure(EnrollmentErrorKind.NotFound, $"Student {studentId} does not exist.");
        }

        var context = new EnrollmentContext(
            course.Capacity, course.EnrolledCount, student.EnrollmentCount, student.AlreadyEnrolled);

        var error = rules
            .Select(rule => rule.Check(context))
            .FirstOrDefault(error => error is not null);

        if (error is not null)
        {
            return EnrollmentResult.Failure(error);
        }

        var enrollment = new CampusFlow.Domain.Models.Enrollment
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAt = timeProvider.GetUtcNow().UtcDateTime
        };

        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        return EnrollmentResult.Success(new EnrollmentResponse(
            courseId, studentId, $"{student.FirstName} {student.LastName}", enrollment.EnrolledAt));
    }
}
