# Online Mathematics Consultation Platform

A comprehensive platform for managing mathematics consultations between teachers and students, built with modern .NET technologies.

## Technologies Used

- **Backend**: ASP.NET Core 8 WebAPI with Entity Framework Core 8
- **Frontend**: Blazor WebAssembly SPA
- **Authentication**: JWT tokens with refresh token mechanism
- **Database**: SQL Server with Code-First approach
- **Real-time Notifications**: SignalR
- **Testing**: xUnit with integration tests
- **Design Patterns**: Factory, Strategy, Observer, Adapter, Repository

## Features

- User authentication and role-based authorization (Admin, Teacher, Student)
- Consultation management (creation, booking, cancellation)
- Learning materials uploading and sharing
- Review system for consultations
- Real-time notifications
- Statistical reports and analytics
- Soft-delete functionality
- Caching for frequently accessed data

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- SQL Server (Local DB or full instance)
- Visual Studio 2022, VS Code, or Rider

### Database Setup

1. Update the connection string in `WebAPI/appsettings.json` and `DataAccess/appsettings.json`
2. Open a terminal in the solution directory and run:
   ```
   cd WebAPI
   dotnet ef database update
   ```

### Running the Application

1. Start the WebAPI project:
   ```
   cd WebAPI
   dotnet run
   ```

2. In a separate terminal, start the Blazor client:
   ```
   cd BlazorClient
   dotnet run
   ```

3. Access the application at https://localhost:7001

### Default Accounts

The system comes with pre-seeded accounts for testing:

- **Admin**: admin@mathconsult.com / Admin123!
- **Teacher**: emma.johnson@mathconsult.com / Teacher123!
- **Student**: alex.brown@mathconsult.com / Student123!

## Authentication Flow

1. User logs in with email/password
2. Server validates credentials and returns JWT + refresh token
3. JWT token is stored in browser localStorage
4. Refresh token is used to obtain new JWT when the original expires
5. All authenticated API requests include JWT in Authorization header

## Role-Based Access Control

The application implements a robust role-based access control system:

- **Anonymous**: Can view public consultations and teacher profiles
- **Students**: Can book consultations, upload materials, leave reviews
- **Teachers**: Can create and manage consultations, upload materials
- **Administrators**: Full access to all resources, user management

Authorization is enforced at multiple levels:
- API endpoints using [Authorize] and [AuthorizeRoles] attributes
- Business logic with explicit role checks
- Blazor UI with conditional rendering based on roles

## Project Structure

- **DataAccess**: Database context, migrations, entities (Models)
- **BusinessLayer**: Business logic, services, DTOs, patterns
- **WebAPI**: Controllers, middleware, configuration
- **BlazorClient**: Blazor WebAssembly frontend
- **Tests**: Unit and integration tests

## Design Patterns

- **Factory Pattern**: UserFactory for creating different user types
- **Strategy Pattern**: RatingStrategies for calculating teacher ratings
- **Observer Pattern**: NotificationObserver for consultation events
- **Adapter Pattern**: Email service adapters for different providers
- **Repository Pattern**: Data access abstraction

## Notable Features

### Soft Delete
Entities implement ISoftDelete interface, allowing for data recovery and maintaining referential integrity.

### Real-time Notifications
SignalR is used to deliver instant notifications to users when:
- New consultations are created
- Consultations are updated or cancelled
- Materials are uploaded
- Reviews are submitted

### Caching
Memory caching is implemented for:
- Teacher listings
- Public consultation data
- Statistics and reports

### Async/Await
All database operations use async/await to maximize performance and scalability.

## Testing

Run tests using:
```
dotnet test
```

Test coverage includes:
- Authentication workflows
- Role-based authorization
- Consultation management
- Strategy pattern implementations
- Observer notifications

## License

This project is licensed under the MIT License - see the LICENSE file for details.
