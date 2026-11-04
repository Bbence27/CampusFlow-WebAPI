using System.ComponentModel.DataAnnotations;

namespace CampusFlow.Contracts.Courses;

public record CourseResponse(int Id, string Code, string Title, int Credits, int Capacity, int EnrolledCount);

public record CreateCourseRequest(
    [Required, StringLength(10, MinimumLength = 3)] string Code,
    [Required, StringLength(100)] string Title,
    [Range(1, 30)] int Credits,
    [Range(1, 500)] int Capacity);

public record UpdateCourseRequest(
    [Required, StringLength(100)] string Title,
    [Range(1, 30)] int Credits,
    [Range(1, 500)] int Capacity);
