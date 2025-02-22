using System.Reflection;
using CourseManagementAPI.Services;
using CourseManagementAPI.Interfaces;
using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using LiteDB;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddLogging();

// Configure LiteDB
builder.Services.AddSingleton<ILiteDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("LiteDB") ?? "Filename=Data/courses.db;Mode=Shared";
    return new LiteDatabase(connectionString);
});

builder.Services.AddScoped<ILiteCollection<Course>>(sp =>
{
    var database = sp.GetRequiredService<ILiteDatabase>();
    return database.GetCollection<Course>("courses");
});

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course Management API", Version = "v1" });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Management API v1"));
}

app.Map("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    return Results.Problem(detail: exception?.Message, statusCode: 500);
});

// REST API endpoints
app.MapPost("/v1/courses", async ([FromServices] CourseService courseService, [FromBody] Course course, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Adding new course: {Title}", course.Title);
        var addedCourse = await courseService.AddCourseAsync(course);
        if (addedCourse != null)
        {
            logger.LogInformation("Course added successfully: {Title}", addedCourse.Title);
            return Results.Ok(new { Message = "Course added successfully", CourseId = addedCourse.Id });
        }
        else
        {
            logger.LogWarning("Failed to add course: {Title}", course.Title);
            return Results.BadRequest(new { Message = "Failed to add course" });
        }
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning(ex, "Invalid course data: {Message}", ex.Message);
        return Results.BadRequest(new { Message = ex.Message });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error adding course");
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
})
.WithName("AddCourse")
.WithOpenApi(operation => new OpenApiOperation(operation)
{
    Summary = "Adds a new course",
    Description = "Adds a new course to the system. The course title and description must not be empty. The title must be 200 characters or less. The course type must be either 'public' or 'private' (case-insensitive)."
});

app.MapGet("/v1/courses", async ([FromServices] CourseService courseService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Retrieving courses");
        var courses = await courseService.GetAllCoursesAsync();
        logger.LogInformation("Retrieved {Count} courses", courses.Count());
        return Results.Ok(courses);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error retrieving courses");
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
})
.WithName("GetAllCourses")
.WithOpenApi(operation => new OpenApiOperation(operation)
{
    Summary = "Retrieves all courses",
    Description = "Gets a list of all courses in the system."
});

app.MapGet("/v1/courses/top5", async ([FromServices] CourseService courseService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Retrieving top 5 recently added courses");
        var courses = await courseService.GetTop5RecentlyAddedCourses();
        var formattedCourses = courses.Select(c => new
        {
            Title = c.Title,
            Description = c.Description,
            CourseCode = c.CourseCode,
            AddedOn = c.AddedOn
        });
        logger.LogInformation("Retrieved {Count} courses", formattedCourses.Count());
        return Results.Ok(formattedCourses);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error retrieving top 5 courses");
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
})
.WithName("GetTop5Courses")
.WithOpenApi(operation => new OpenApiOperation(operation)
{
    Summary = "Retrieves top 5 recently added courses",
    Description = "Gets a list of the 5 most recently added courses, ordered by title. The response includes the course title, description, course code, and the date it was added."
});

app.MapGet("/", () => "Course Management API: Use REST API endpoints or Swagger UI to interact with the service.")
.WithName("Root")
.WithOpenApi(operation => new OpenApiOperation(operation)
{
    Summary = "API Root",
    Description = "Returns a welcome message for the Course Management API."
});

app.Run();
