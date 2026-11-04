using CampusFlow.Contracts.Enrollments;

namespace CampusFlow.Services.Enrollment;

public record EnrollmentResult(EnrollmentResponse? Enrollment, EnrollmentError? Error)
{
    public static EnrollmentResult Success(EnrollmentResponse enrollment) => new(enrollment, null);

    public static EnrollmentResult Failure(EnrollmentError error) => new(null, error);

    public static EnrollmentResult Failure(EnrollmentErrorKind kind, string message) =>
        Failure(new EnrollmentError(kind, message));
}
