using Moq;
using LiteDB;
using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;


namespace CourseManagementAPI.Tests
{
    public class CourseRepositoryTests
    {
        private readonly Mock<ILiteCollection<Course>> _courseCollectionMock;
        private readonly CourseRepository _courseRepository;

        public CourseRepositoryTests()
        {
            _courseCollectionMock = new Mock<ILiteCollection<Course>>();
            _courseRepository = new CourseRepository(_courseCollectionMock.Object);
        }

        [Fact]
        public async Task AddCourseAsync_Should_Call_Insert()
        {
            // Arrange
            var course = new Course { Title = "Test Course", CourseCode = "TEST101", Type = "Public" };
            _courseCollectionMock.Setup(x => x.Insert(It.IsAny<Course>())).Returns(new BsonValue(course.Id));

            // Act
            var result = await _courseRepository.AddCourseAsync(course);

            // Assert
            Assert.True(result);
            _courseCollectionMock.Verify(x => x.Insert(It.Is<Course>(c => c.Title == course.Title && c.CourseCode == course.CourseCode)), Times.Once);
        }

        [Fact]
        public async Task GetAllCoursesAsync_Should_Return_All_Courses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { Title = "Course 1", CourseCode = "C001", Type = "Public" },
                new Course { Title = "Course 2", CourseCode = "C002", Type = "Private" }
            };
            _courseCollectionMock.Setup(x => x.FindAll()).Returns(courses);

            // Act
            var result = await _courseRepository.GetAllCoursesAsync();

            // Assert
            Assert.Equal(courses, result);
            _courseCollectionMock.Verify(x => x.FindAll(), Times.Once);
        }
    }
}
