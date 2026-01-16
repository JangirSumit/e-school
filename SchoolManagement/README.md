# School Management System - MAUI .NET 9

Multi-tenant School Management System built with .NET MAUI 9.0

## Features

- **Multi-Tenant Architecture**: Each school operates independently with isolated data
- **Authentication**: School signup and user login
- **Student Management**: Add and manage students
- **Faculty Management**: Manage teaching staff
- **Attendance Tracking**: Record and monitor attendance
- **Marks Management**: Track student performance
- **Holiday Calendar**: Manage school holidays

## Architecture

- **MVVM Pattern**: Clean separation of concerns
- **Dependency Injection**: Services registered in MauiProgram.cs
- **Multi-tenancy**: TenantId isolation across all entities
- **In-Memory Data Store**: Can be replaced with SQLite/API

## Project Structure

```
SchoolManagement/
├── Models/          # Data models (Tenant, User, Student, Faculty, etc.)
├── Services/        # Business logic (AuthService)
├── ViewModels/      # MVVM ViewModels
├── Views/           # XAML Pages
├── Data/            # Data access layer
└── Helpers/         # Utilities (SessionManager, Converters)
```

## Getting Started

1. Open solution in Visual Studio 2022
2. Select target platform (Android/iOS/Windows)
3. Run the application
4. Sign up a new school
5. Login with school credentials

## Default Flow

1. **Signup**: School registers with admin account
2. **Login**: Admin logs in
3. **Dashboard**: Access to all modules
4. **Manage**: Students, Faculty, Attendance, Marks, Holidays

## Tech Stack

- .NET 9.0
- .NET MAUI
- C# 12
- XAML

## Next Steps

- Implement SQLite for persistent storage
- Add REST API integration
- Implement complete CRUD operations
- Add reporting features
- Implement role-based access control
- Add push notifications
