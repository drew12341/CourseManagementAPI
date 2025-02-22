

namespace CourseManagementAPI.Models
{
    /// <summary>
    /// Represents the type of a course.
    /// </summary>
    /// <remarks>
    /// This enum is serialized as a string in API responses.
    /// Valid values are "public" and "private" (case-insensitive).
    /// </remarks>
    public enum CourseType
    {
        Public,
        Private
    }

    /// <summary>
    /// Represents a course in the Course Management system.
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Gets the unique identifier for the course.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the course.
        /// </summary>
        public string? Description { get; set; }

        private CourseType _type;

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public string Type
        {
            get => _type.ToString();
            set
            {
                if (Enum.TryParse<CourseType>(value, true, out var parsedType))
                {
                    _type = parsedType;
                }
                else
                {
                    throw new ArgumentException("Invalid course type. Must be either 'public' or 'private'.");
                }
            }
        }

        /// <summary>
        /// Gets the enum value of the course type.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public CourseType TypeEnum => _type;

        /// <summary>
        /// Gets or sets the unique course code.
        /// </summary>
        public required string CourseCode { get; set; }

        /// <summary>
        /// Gets the date and time when the course was added.
        /// </summary>
        public DateTime AddedOn { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Course class.
        /// </summary>
        public Course()
        {
            RegenerateId();
        }

        /// <summary>
        /// Regenerates the Id for the course.
        /// </summary>
        public void RegenerateId()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Sets the AddedOn property to the current UTC time.
        /// This method should be called when the course is being added to the database.
        /// </summary>
        public void SetAddedOnToNow()
        {
            AddedOn = DateTime.UtcNow;
        }
    }
}
