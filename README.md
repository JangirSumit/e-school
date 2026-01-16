# School Management System - Complete Solution

Multi-tenant School Management System with MAUI Mobile App and ASP.NET Core API

## Projects

### 1. SchoolManagement (MAUI Mobile App)
- **Location**: `./SchoolManagement/`
- **Framework**: .NET MAUI 9.0
- **Platforms**: Android, iOS, Windows, macOS
- **Architecture**: MVVM Pattern
- **Features**:
  - School signup and login
  - Student management
  - Faculty management
  - Attendance tracking
  - Marks management
  - Holiday calendar
  - Multi-tenant support

### 2. SchoolManagement.API (Backend API)
- **Location**: `./SchoolManagement.API/`
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer Token
- **Features**:
  - RESTful API endpoints
  - Multi-tenant data isolation
  - Swagger/OpenAPI documentation
  - Entity Framework Code-First
  - JWT authentication

## Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or VS Code
- SQL Server (LocalDB or full instance)
- Android/iOS SDK (for mobile development)

### Setup API Server

1. Navigate to API project:
```bash
cd SchoolManagement.API
```

2. Update connection string in `appsettings.json`

3. Create database:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Run API:
```bash
dotnet run
```

API will be available at `https://localhost:7xxx`

### Setup Mobile App

1. Navigate to MAUI project:
```bash
cd SchoolManagement
```

2. Update API base URL in app configuration

3. Run on desired platform:
```bash
dotnet build -t:Run -f net9.0-android
```

## Architecture

### Multi-Tenancy
- Each school is a separate tenant
- TenantId in all database entities
- JWT token contains TenantId claim
- Automatic tenant filtering in API

### Authentication Flow
1. School signs up → Creates Tenant + Admin User
2. User logs in → Receives JWT token
3. Token contains: UserId, TenantId, Role
4. All API calls include token in Authorization header
5. API validates token and filters data by TenantId

### Database Schema
- **Tenants** - School information
- **Users** - Admin, Faculty, Student users
- **Students** - Student details
- **Faculties** - Faculty details
- **Attendance** - Daily attendance records
- **Marks** - Exam marks and grades
- **Holidays** - School holiday calendar

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/login` | POST | User login |
| `/api/auth/signup` | POST | School registration |
| `/api/students` | GET | Get all students |
| `/api/students` | POST | Create student |
| `/api/faculties` | GET | Get all faculties |
| `/api/attendance` | GET | Get attendance |
| `/api/marks/student/{id}` | GET | Get student marks |
| `/api/holidays` | GET | Get holidays |

## Technology Stack

### Mobile App
- .NET MAUI 9.0
- MVVM Pattern
- Dependency Injection
- XAML UI

### Backend API
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

## Next Steps

1. **Database Migration**: Replace in-memory store with API calls in mobile app
2. **Security**: Implement BCrypt for password hashing
3. **Features**: Add more modules (fees, exams, timetable, etc.)
4. **UI/UX**: Enhance mobile app design
5. **Notifications**: Push notifications for attendance, marks
6. **Reports**: Generate PDF reports
7. **Analytics**: Dashboard with charts and statistics

## License

MIT License
