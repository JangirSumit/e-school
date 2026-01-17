# API Deployment Guide

## Versioning & Release

### Create a new release:
```bash
git tag v1.0.0
git push origin v1.0.0
```

GitHub Actions will automatically:
- Build the API
- Create release artifact
- Publish to GitHub Releases

### Manual trigger:
1. Go to GitHub Actions
2. Select "Deploy API" workflow
3. Click "Run workflow"
4. Enter version number

## Download & Deploy

### 1. Download Release
- Go to GitHub Releases
- Download `api-v1.0.0` artifact
- Extract to server

### 2. Configure
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=schoolmanagement.db"
  },
  "Jwt": {
    "Key": "YOUR-SECRET-KEY-HERE",
    "Issuer": "SchoolManagementAPI",
    "Audience": "SchoolManagementApp"
  }
}
```

### 3. Run
```bash
dotnet SchoolManagement.API.dll
```

## Requirements
- .NET 9.0 Runtime
- Port 8080 (or configure in appsettings)
