# CourseManagementAPI

CourseManagementAPI is a .NET Core application that provides functionality for managing courses through REST API endpoints.

## Features

- Add new courses
- Retrieve all courses
- Retrieve the top 5 recently added courses, ordered by title

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code

## Building and Running the Project

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/CourseManagementAPI.git
   cd CourseManagementAPI
   ```

2. Build the project:
   ```
   dotnet build
   ```

3. Run the project:
   ```
   dotnet run --project CourseManagementAPI
   ```

The API will start running on `https://localhost:7052` and `http://localhost:5291` by default. These ports can be configured in the `Properties/launchSettings.json` file.

## API Endpoints

### REST Endpoints

- POST /v1/courses: Add a new course
- GET /v1/courses: Get all courses
- GET /v1/courses/top5: Get top 5 recently added courses, ordered by title

You can use Swagger UI to test these endpoints. Once the application is running, navigate to `https://localhost:7052/swagger` in your web browser.

## Running Tests

To run the unit tests:

```
dotnet test
```

## Configuration

Application configuration can be found in `appsettings.json`. Modify this file to change application settings.

## Contributing

Please read CONTRIBUTING.md for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.
