namespace CampusFlow.Domain.Models;

    using System;
    using System.Collections.Generic;

    public partial class SeedData
    {
        public List<Course> Courses { get; set; } = [];
        public List<Student> Students { get; set; } = [];
        public List<Enrollment> Enrollments { get; set; } = [];
    }

    public partial class Course
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
        public int Capacity { get; set; }
        public List<Enrollment> Enrollments { get; set; } = [];
    }

    public partial class Student
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<Enrollment> Enrollments { get; set; } = [];
    }

    public partial class Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public DateTime EnrolledAt { get; set; }
    }
