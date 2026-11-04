namespace CampusFlow.Services.Enrollment;

public interface IEnrollmentService
{
    Task<EnrollmentResult> EnrollAsync(int courseId, int studentId);
}
