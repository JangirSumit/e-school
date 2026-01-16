# School Management API - .NET 9

ASP.NET Core Web API with Entity Framework Core and SQL Server for multi-tenant School Management System.

## Features

- **JWT Authentication** - Secure token-based authentication
- **Multi-Tenant Architecture** - Complete tenant isolation
- **Entity Framework Core** - Code-first database approach
- **RESTful API** - Standard HTTP methods (GET, POST, PUT, DELETE)
- **Swagger/OpenAPI** - Interactive API documentation

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/signup` - School registration

### Students
- `GET /api/students` - Get all students (tenant-specific)
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

### Faculties
- `GET /api/faculties` - Get all faculties
- `POST /api/faculties` - Create faculty

### Attendance
- `GET /api/attendance?date={date}` - Get attendance by date
- `POST /api/attendance` - Mark attendance

### Marks
- `GET /api/marks/student/{studentId}` - Get marks by student
- `POST /api/marks` - Add marks

### Holidays
- `GET /api/holidays` - Get all holidays
- `POST /api/holidays` - Create holiday

## Database Setup

1. Update connection string in `appsettings.json`
2. Run migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Run the API

```bash
dotnet run
```

API will be available at:
- HTTPS: https://localhost:7xxx
- HTTP: http://localhost:5xxx
- Swagger UI: https://localhost:7xxx/swagger

## Configuration

Update `appsettings.json`:
- **ConnectionStrings:DefaultConnection** - SQL Server connection
- **Jwt:Key** - Secret key for JWT (change in production)
- **Jwt:Issuer** - Token issuer
- **Jwt:Audience** - Token audience

## Security

- All endpoints (except auth) require JWT token
- Multi-tenant isolation via TenantId in JWT claims
- Password hashing (use BCrypt in production)

## Tech Stack

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9.0
- SQL Server
- JWT Bearer Authentication
- Swagger/OpenAPI
