PPROJECT:
-
-
-

Online Mathematics Consultation Platform - Development Plan:

1) Architecture & Initialization
   - Create solutions: DataAccess, BusinessLayer, and WebAPI (3-layer).
   - Implement TPH for User hierarchy (Student, Teacher, Admin).
   - Configure EF Core (DbContext, migrations, seed data).
   - Add README + error handling middleware (global exception handler).
   - PDF Criteria: [0.5p] README/Errors, [1p] OOP (User classes, etc.), [0.8p] SOLID (DI, separation), [1p] EF (migrations, loading), [0.9p] Code Quality, [1.4p] Web API CRUD, [0.5p] Architecture boundaries.

2) Core Features (Consultations, Materials, Reviews)
   - Entities: Consultation, Material, Review, plus many-to-many (ConsultationStudent).
   - Services and controllers for scheduling consultations, uploading materials, posting reviews.
   - LINQ queries for filtering, sorting, grouping, joining.
   - PDF Criteria: [0.7p] LINQ, [0.3p] Lambda, [1p] EF relations, [1.4p] Web API (CRUD).

3) Notifications, Statistics, Security
   - Notification entity + in-app notifications (Observer pattern for events).
   - Basic statistics (average ratings, teacher popularity) with LINQ (GroupBy, etc.).
   - JWT authentication & role-based authorization.
   - Patterns: Factory (create user types), Strategy (rating calculation), Observer (notifications).
   - Cache for teacher lists, rate limiting for public endpoints.
   - PDF Criteria: [0.9p] Patterns, [1.4p] WebAPI (caching, rate limiting).

4) Testing & Refinements
   - Unit tests for services, integration tests for controllers.
   - Code review, refactoring, final README updates.
   - PDF Criteria: [0.9p] Code Quality, [0.8p] SOLID final check, plus finishing error handling.

5) Bonus: Blazor Client
   - Single-page client with pages: Login, Dashboard, Consultations, Create Consultation (Teacher), Profile.
   - Use HttpClient for calling WebAPI, store JWT in local storage.
   - Show real-time notifications (optional SignalR).
   - PDF Criteria: [1p] (Bonus) Blazor integration.

README Instructions:

1) Install .NET 8 SDK
2) From /Proiect/DataAccess run:
   dotnet ef migrations add Initial
   dotnet ef database update
3) dotnet build /Proiect.sln
4) Run WebAPI and BlazorClient projects