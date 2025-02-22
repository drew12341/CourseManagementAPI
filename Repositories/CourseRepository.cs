using CourseManagementAPI.Interfaces;
using CourseManagementAPI.Models;
using LiteDB;


namespace CourseManagementAPI.Repositories
{
public class CourseRepository : ICourseRepository, IDisposable
{
    private readonly ILiteCollection<Course> _courses;
    private bool _disposed = false;

    public CourseRepository(ILiteCollection<Course> courses)
    {
        _courses = courses ?? throw new ArgumentNullException(nameof(courses));
    }

    public async Task<bool> AddCourseAsync(Course course)
    {
        return await Task.FromResult(_courses.Insert(course) != null);
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await Task.FromResult(_courses.FindAll());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Add any cleanup code here if needed
            }

            _disposed = true;
        }
    }
}
}
