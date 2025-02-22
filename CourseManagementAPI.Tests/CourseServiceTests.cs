using Moq;
using CourseManagementAPI.Services;
using CourseManagementAPI.Models;
using CourseManagementAPI.Interfaces;


namespace CourseManagementAPI.Tests
{
    public class CourseServiceTests
    {
        [Fact]
        public async Task AddCourse_WithValidData_ShouldSucceed()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = "Valid Title",
                Description = "Valid Description",
                Type = "public",
                CourseCode = "TEST-001"
            };

            mockRepository.Setup(repo => repo.AddCourseAsync(It.IsAny<Course>()))
                .ReturnsAsync(true);

            // Act
            var result = await courseService.AddCourseAsync(course);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(course.Title, result.Title);
            Assert.Equal(course.Description, result.Description);
            Assert.Equal(course.Type, result.Type);
            Assert.Equal(course.CourseCode, result.CourseCode);
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task AddCourse_WithInvalidTitle_ShouldThrowException()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = new string('A', 201), // 201 characters
                Description = "Valid Description",
                Type = "public",
                CourseCode = "TEST-002"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => courseService.AddCourseAsync(course));
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AddCourse_WithEmptyTitle_ShouldThrowException()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = "",
                Description = "Valid Description",
                Type = "public",
                CourseCode = "TEST-003"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => courseService.AddCourseAsync(course));
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AddCourse_WithEmptyDescription_ShouldThrowException()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = "Valid Title",
                Description = "",
                Type = "public",
                CourseCode = "TEST-004"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => courseService.AddCourseAsync(course));
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AddCourse_WithInvalidType_ShouldThrowException()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = "Valid Title",
                Description = "Valid Description",
                CourseCode = "TEST-005"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            {
                course.Type = "invalid"; // Set invalid type here
                return courseService.AddCourseAsync(course);
            });
            Assert.Equal("Invalid course type. Must be either 'public' or 'private'.", exception.Message);
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AddCourse_FailureToSave_ShouldReturnNull()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var course = new Course
            {
                Title = "Valid Title",
                Description = "Valid Description",
                Type = "private",
                CourseCode = "TEST-006"
            };

            mockRepository.Setup(repo => repo.AddCourseAsync(It.IsAny<Course>()))
                .ReturnsAsync(false);

            // Act
            var result = await courseService.AddCourseAsync(course);

            // Assert
            Assert.Null(result);
            mockRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCourses_ShouldReturnAllCourses()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            var courses = new List<Course>
            {
                new Course { Title = "Course 1", Description = "Description 1", Type = "public", CourseCode = "TEST-007" },
                new Course { Title = "Course 2", Description = "Description 2", Type = "private", CourseCode = "TEST-008" }
            };

            mockRepository.Setup(repo => repo.GetAllCoursesAsync())
                .ReturnsAsync(courses);

            // Act
            var result = await courseService.GetAllCoursesAsync();

            // Assert
            Assert.Equal(courses.Count, result.Count());
            Assert.Equal(courses, result);
            mockRepository.Verify(repo => repo.GetAllCoursesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTop5RecentlyAddedCourses_ShouldReturnCorrectResult()
        {
            // Arrange
            var mockRepository = new Mock<ICourseRepository>();
            var courseService = new CourseService(mockRepository.Object);
            
            var courses = new List<Course>
            {
                new Course { Title = "Python Programming", Description = "Learn Python", CourseCode = "TEST-008", Type = "Public" },
                new Course { Title = "Java Fundamentals", Description = "Java basics", CourseCode = "TEST-009", Type = "Public" },
                new Course { Title = "C# Advanced", Description = "Advanced C#", CourseCode = "TEST-010", Type = "Private" },
                new Course { Title = "Web Development", Description = "HTML, CSS, JS", CourseCode = "TEST-011", Type = "Public" },
                new Course { Title = "Database Design", Description = "SQL and NoSQL", CourseCode = "TEST-012", Type = "Private" },
                new Course { Title = "Machine Learning", Description = "AI and ML", CourseCode = "TEST-013", Type = "Public" }
            };

            // Set AddedOn for each course
            for (int i = 0; i < courses.Count; i++)
            {
                courses[i].SetAddedOnToNow();
                System.Threading.Thread.Sleep(1); // Ensure unique timestamps
            }

            mockRepository.Setup(repo => repo.GetAllCoursesAsync())
                .ReturnsAsync(courses);

            // Act
            var result = await courseService.GetTop5RecentlyAddedCourses();

            // Assert
            Assert.Equal(5, result.Count());
            
            // Check if all required fields are present and not default
            foreach (var course in result)
            {
                Assert.NotNull(course.Title);
                Assert.NotNull(course.Description);
                Assert.NotNull(course.CourseCode);
                Assert.NotEqual(default, course.AddedOn);
            }

            // Verify the order is correct (ordered by title)
            var orderedTitles = result.Select(c => c.Title).ToList();
            Assert.Equal(orderedTitles.OrderBy(t => t).ToList(), orderedTitles);

            // Verify that the returned courses are the 5 most recently added
            var expectedCourses = courses.OrderByDescending(c => c.AddedOn).Take(5).OrderBy(c => c.Title);
            Assert.Equal(expectedCourses.Select(c => c.Title), result.Select(c => c.Title));
        }
    }
}
