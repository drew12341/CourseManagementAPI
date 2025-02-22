using CourseManagementAPI.Models;

namespace CourseManagementAPI.Interfaces
{
    /// <summary>
    /// Defines the contract for course repository operations.
    /// </summary>
    public interface ICourseRepository
    {
        /// <summary>
        /// Adds a new course to the repository.
        /// </summary>
        /// <param name="course">The course to add.</param>
        /// <returns>True if the course was successfully added; otherwise, false.</returns>
        Task<bool> AddCourseAsync(Course course);

        /// <summary>
        /// Retrieves all courses from the repository.
        /// </summary>
        /// <returns>An enumerable collection of all courses.</returns>
        Task<IEnumerable<Course>> GetAllCoursesAsync();
    }
}
