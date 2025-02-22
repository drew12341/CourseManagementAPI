using CourseManagementAPI.Interfaces;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services
{
    /// <summary>
    /// Provides services for managing courses in the Course Management API.
    /// </summary>
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;
        
        /// <summary>
        /// Initializes a new instance of the CourseService class.
        /// </summary>
        /// <param name="courseRepository">The repository used for course data operations.</param>
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        
        /// <summary>
        /// Adds a new course to the repository.
        /// </summary>
        /// <param name="course">The course to add.</param>
        /// <returns>The added course if successful; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when the course fails validation.</exception>
        public async Task<Course?> AddCourseAsync(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            try
            {
                // Regenerate the course ID
                course.RegenerateId();
                
                // Explicitly set the Type to trigger validation
                course.Type = course.Type;

                ValidateCourse(course);
                course.SetAddedOnToNow();
                bool success = await _courseRepository.AddCourseAsync(course);
                return success ? course : null;
            }
            catch (ArgumentException)
            {
                // Rethrow ArgumentException to ensure it's propagated
                throw;
            }
        }

        /// <summary>
        /// Validates the course object.
        /// </summary>
        /// <param name="course">The course to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the course fails validation.</exception>
        private void ValidateCourse(Course course)
        {
            if (course == null)
            {
                throw new ArgumentException("Course cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(course.Title))
            {
                throw new ArgumentException("Course title is required.");
            }

            if (course.Title.Length > 200)
            {
                throw new ArgumentException("Course title must be 200 characters or less.");
            }

            if (string.IsNullOrWhiteSpace(course.Description))
            {
                throw new ArgumentException("Course description is required.");
            }

            // The Type property now handles its own validation, so we don't need to check it here.
            // The ArgumentException will be thrown if an invalid type is set.
        }
        
        /// <summary>
        /// Retrieves all courses from the repository.
        /// </summary>
        /// <returns>An enumerable collection of all courses.</returns>
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetAllCoursesAsync();
        }

        /// <summary>
        /// Retrieves the top 5 most recently added courses, ordered by title.
        /// </summary>
        /// <returns>An enumerable collection of the top 5 most recently added courses, ordered by title.</returns>
        public async Task<IEnumerable<Course>> GetTop5RecentlyAddedCourses()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            var top5 = courses.OrderByDescending(c => c.AddedOn)
                               .Take(5)
                               .OrderBy(c => c.Title);
            return top5;
        }
    }
}
