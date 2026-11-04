using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CampusFlow.Endpoints;
using CampusFlow.ResourceAccess.Sqlite;
using CampusFlow.Services.Enrollment;
using CampusFlow.Util.Logging;
using CampusFlow.Domain.Models;
using CampusFlow.Rules.Enrollment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAppLogger(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<IEnrollmentRule, NotAlreadyEnrolledRule>();
builder.Services.AddSingleton<IEnrollmentRule, CapacityRule>();
builder.Services.AddSingleton<IEnrollmentRule, MaxCoursesPerStudentRule>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<IEnrollmentService>(sp =>
    new LoggingEnrollmentService(
        sp.GetRequiredService<EnrollmentService>(),
        sp.GetRequiredService<ILogger<LoggingEnrollmentService>>()));
builder.Services.AddDbContext<CampusFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CampusFlow")));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusFlowDbContext>();

    if (db.Database.EnsureCreated())
    {
        var path = Path.Combine(app.Environment.ContentRootPath, "Data", "seed-data.json");
        var seed = JsonSerializer.Deserialize<SeedData>(File.ReadAllText(path), JsonSerializerOptions.Web)!;

        db.Courses.AddRange(seed.Courses);
        db.Students.AddRange(seed.Students);
        db.Enrollments.AddRange(seed.Enrollments);
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapGet("/swagger", () => Results.Content("""
        <!doctype html>
        <html>
        <head>
          <title>CampusFlow API</title>
          <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5.33.1/swagger-ui.css" />
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://unpkg.com/swagger-ui-dist@5.33.1/swagger-ui-bundle.js"></script>
          <script>
            SwaggerUIBundle({ url: "/openapi/v1.json", dom_id: "#swagger-ui" });
          </script>
        </body>
        </html>
        """, "text/html"))
        .ExcludeFromDescription();
}

app.UseExceptionHandler();

var appLogger = app.Services.GetRequiredService<IAppLogger>();
app.Use(async (context, next) =>
{
    await next(context);
    appLogger.Info($"{context.Request.Method} {context.Request.Path}{context.Request.QueryString} -> {context.Response.StatusCode}");
});

app.UseStatusCodePages();
app.UseHttpsRedirection();

app.MapCourseEndpoints();
app.MapStudentEndpoints();
app.MapEnrollmentEndpoints();
app.MapStatisticsEndpoints();

app.Run();
