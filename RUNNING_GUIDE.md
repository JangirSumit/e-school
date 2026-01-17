# School Management System - Running Guide

## Quick Start

### Option 1: Run Both (Easiest)
```bash
run-both.bat
```
This will start both the API and Windows app automatically.

### Option 2: Run Separately

#### Start API Server
```bash
cd SchoolManagement.API
dotnet run
```
API will be available at: `https://localhost:7001`

#### Start Windows App
```bash
cd SchoolManagement
dotnet run -f net9.0-windows10.0.19041.0
```

### Option 3: Visual Studio Code
1. Open the workspace in VS Code
2. Press F5 or select "Launch API + Windows App" from debug menu
3. Both applications will start with debugging enabled

## Features Implemented

### ✅ School Registration
- Schools can register themselves with admin account
- Multi-tenant architecture (each school is isolated)
- BCrypt password hashing for security

### ✅ Authentication
- JWT token-based authentication
- Secure login/logout
- Session management

### ✅ Student Management
- Add students with user accounts
- View all students (filtered by school/tenant)
- Delete students
- Each student gets login credentials

### ✅ API Endpoints

#### Auth
- `POST /api/auth/signup` - Register new school
- `POST /api/auth/login` - Login

#### Students
- `GET /api/students` - Get all students (tenant-filtered)
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `DELETE /api/students/{id}` - Delete student

## Database

- **Type**: SQLite (for easy setup)
- **Location**: `SchoolManagement.API/schoolmanagement.db`
- **Migrations**: Already applied

## Configuration

### API Configuration
- **File**: `SchoolManagement.API/appsettings.json`
- **Connection String**: SQLite database
- **JWT Settings**: Token expiration, secret key

### MAUI App Configuration
- **File**: `SchoolManagement/Services/ApiConfig.cs`
- **Debug**: Uses `https://localhost:7001`
- **Production**: Update `BaseUrl` for production API

## Testing the Application

### 1. Register a School
1. Launch the Windows app
2. Click "Sign Up School"
3. Fill in school details:
   - School Name
   - Email
   - Phone
   - Address
   - Admin Name
   - Password
4. Click "Sign Up"

### 2. Login
1. Use the email and password from registration
2. Click "Login"
3. You'll see the dashboard

### 3. Add Students
1. From dashboard, click "Students"
2. Click "Add Student" button
3. Fill in student details:
   - Full Name
   - Email
   - Phone
   - Roll Number
   - Class
   - Section
   - Parent Name
   - Parent Phone
   - Password (for student login)
4. Student will be created with a user account

### 4. View Students
- All students are displayed in a list
- Shows: Name, Roll Number, Class, Section
- Filtered automatically by your school (tenant)

## Architecture

### Multi-Tenancy
- Each school is a separate tenant
- All data is isolated by TenantId
- JWT token contains TenantId claim
- API automatically filters data by tenant

### Security
- BCrypt password hashing
- JWT token authentication
- HTTPS communication
- Token stored in session

### Data Flow
```
MAUI App → ApiService → API Controller → DbContext → SQLite
```

## Next Steps

### To Add More Features:
1. **Faculty Management**: Similar to students
2. **Attendance**: Mark and view attendance
3. **Marks**: Enter and view exam marks
4. **Holidays**: Manage school holidays
5. **Reports**: Generate PDF reports
6. **Notifications**: Push notifications

### For Production:
1. Update `ApiConfig.BaseUrl` in MAUI app
2. Deploy API to cloud (Azure, AWS, etc.)
3. Use SQL Server or PostgreSQL instead of SQLite
4. Add proper error logging
5. Implement refresh tokens
6. Add data validation
7. Implement proper authorization

## Troubleshooting

### API won't start
- Check if port 7001 is available
- Run `dotnet restore` in API folder
- Check database file permissions

### App can't connect to API
- Ensure API is running
- Check firewall settings
- Verify API URL in `ApiConfig.cs`
- For Android: Use `10.0.2.2:7001` instead of `localhost:7001`

### Database errors
- Delete `schoolmanagement.db` and restart API
- Run `dotnet ef database update` in API folder

## API Documentation

When API is running, visit:
- Swagger UI: `https://localhost:7001/swagger`
- API endpoints documentation available there
