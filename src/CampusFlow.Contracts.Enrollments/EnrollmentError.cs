namespace CampusFlow.Contracts.Enrollments;

public enum EnrollmentErrorKind
{
    NotFound,
    Conflict,
    BadRequest
}

public record EnrollmentError(EnrollmentErrorKind Kind, string Message);