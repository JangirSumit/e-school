# Android Testing Guide

## Quick Start

### Option 1: Automated Setup (Easiest)
```bash
run-android.bat
```
This will:
- Build and start the API
- Configure ADB port forwarding
- Build and deploy Android app
- Show connection instructions

### Option 2: Manual Setup

#### Step 1: Start API
```bash
cd SchoolManagement.API
dotnet run
```

#### Step 2: Setup Port Forwarding (Real Device Only)
```bash
setup-adb-forwarding.bat
```

#### Step 3: Build and Deploy Android App
```bash
cd SchoolManagement
dotnet build -t:Run -f net9.0-android
```

## Connection Methods

### 🟢 Android Emulator (Automatic)
- **API URL**: `http://10.0.2.2:7001`
- **Setup**: None required
- **How it works**: `10.0.2.2` is a special alias to host machine's localhost

### 🟢 Real Device via USB (Recommended)
- **API URL**: `http://localhost:7001`
- **Setup Required**: ADB port forwarding
- **Command**: `adb reverse tcp:7001 tcp:7001`
- **Requirements**:
  - USB Debugging enabled
  - Device connected via USB
  - Device authorized for debugging

### 🟡 Real Device via WiFi (Advanced)
- **API URL**: `http://YOUR_PC_IP:7001`
- **Setup Required**: 
  1. Find your PC's IP address: `ipconfig`
  2. Update `ApiConfig.cs` with your IP
  3. Allow port 7001 in Windows Firewall
- **Example**: `http://192.168.1.100:7001`

## Troubleshooting

### ADB Not Found
**Problem**: `adb is not recognized as an internal or external command`

**Solution**:
1. Install Android SDK Platform Tools
2. Download from: https://developer.android.com/tools/releases/platform-tools
3. Add to PATH:
   - Windows: `C:\Users\YourName\AppData\Local\Android\Sdk\platform-tools`
   - Or install via Android Studio

### Device Not Detected
**Problem**: `adb devices` shows no devices

**Solutions**:
1. Enable USB Debugging:
   - Settings → About Phone → Tap "Build Number" 7 times
   - Settings → Developer Options → Enable USB Debugging
2. Accept authorization prompt on device
3. Try different USB cable (data cable, not charge-only)
4. Restart ADB: `adb kill-server` then `adb start-server`

### Port Forwarding Failed
**Problem**: `adb reverse` command fails

**Solutions**:
1. Check device is connected: `adb devices`
2. Device must be authorized (check phone screen)
3. Try: `adb reverse --remove-all` then setup again
4. Restart ADB server

### Cannot Connect to API
**Problem**: App shows "Failed to connect" or timeout

**For Emulator**:
- ✓ API must be running on PC
- ✓ Use `http://10.0.2.2:7001` (not localhost)
- ✓ Check API is accessible: Open browser on PC → `https://localhost:7001/swagger`

**For Real Device (USB)**:
- ✓ Port forwarding must be active: `adb reverse tcp:7001 tcp:7001`
- ✓ Check with: `adb reverse --list`
- ✓ API must be running

**For Real Device (WiFi)**:
- ✓ PC and phone on same WiFi network
- ✓ Windows Firewall allows port 7001
- ✓ Use PC's IP address, not localhost
- ✓ Test from phone browser: `http://YOUR_PC_IP:7001/swagger`

### SSL/HTTPS Issues
**Problem**: Certificate errors or SSL handshake failures

**Solution**: App is configured to accept self-signed certificates in debug mode
```csharp
// Already configured in ApiService.cs
ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
```

### Build Errors
**Problem**: Android build fails

**Solutions**:
1. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build -f net9.0-android
   ```
2. Check Android SDK is installed
3. Check Java JDK is installed
4. Update Android SDK via Android Studio

## Testing Checklist

### Before Testing
- [ ] API server is running
- [ ] Device/Emulator is connected
- [ ] Port forwarding configured (if using real device)
- [ ] USB Debugging enabled (if using real device)

### Test Scenarios
1. **School Registration**
   - [ ] Open app
   - [ ] Click "Sign Up School"
   - [ ] Fill all fields
   - [ ] Submit and verify success

2. **Login**
   - [ ] Use registered credentials
   - [ ] Verify dashboard loads
   - [ ] Check welcome message shows

3. **Add Student**
   - [ ] Navigate to Students
   - [ ] Click "Add Student"
   - [ ] Fill student details
   - [ ] Verify student appears in list

4. **View Students**
   - [ ] Students list loads
   - [ ] Shows correct data
   - [ ] Filtered by school (tenant)

## Useful Commands

### ADB Commands
```bash
# List connected devices
adb devices

# Setup port forwarding
adb reverse tcp:7001 tcp:7001

# Check active port forwarding
adb reverse --list

# Remove specific port forwarding
adb reverse --remove tcp:7001

# Remove all port forwarding
adb reverse --remove-all

# Restart ADB server
adb kill-server
adb start-server

# Install APK manually
adb install path/to/app.apk

# View device logs
adb logcat

# View app logs only
adb logcat | findstr "SchoolManagement"
```

### Build Commands
```bash
# Build Android app
dotnet build -f net9.0-android

# Build and deploy
dotnet build -t:Run -f net9.0-android

# Build release version
dotnet build -f net9.0-android -c Release

# Clean build
dotnet clean
```

### API Commands
```bash
# Start API
cd SchoolManagement.API
dotnet run

# Start API with watch (auto-reload)
dotnet watch run

# Check API is running
curl https://localhost:7001/swagger
```

## Network Configuration

### Find Your PC's IP Address
```bash
# Windows
ipconfig

# Look for "IPv4 Address" under your active network adapter
# Example: 192.168.1.100
```

### Configure Firewall (for WiFi testing)
```bash
# Windows Firewall - Allow port 7001
netsh advfirewall firewall add rule name="School Management API" dir=in action=allow protocol=TCP localport=7001
```

### Test API from Phone Browser
1. Open Chrome on Android
2. Navigate to: `http://YOUR_PC_IP:7001/swagger`
3. If it loads, API is accessible
4. If not, check firewall and network

## Performance Tips

### For Faster Deployment
1. Use USB 3.0 port
2. Enable "Install via USB" in Developer Options
3. Keep device screen on during deployment
4. Use Release build for final testing

### For Better Debugging
1. Enable "Stay awake" in Developer Options
2. Use `adb logcat` to view logs
3. Install Android Studio for better debugging tools
4. Use Chrome DevTools for web debugging

## Production Deployment

### Before Publishing
1. Update `ApiConfig.cs` with production URL
2. Build in Release mode
3. Sign the APK
4. Test on multiple devices
5. Test on different Android versions

### Build Release APK
```bash
dotnet publish -f net9.0-android -c Release
```

APK location: `bin/Release/net9.0-android/publish/`

## Support

### Common Issues
- **Slow deployment**: Use USB 3.0, enable "Install via USB"
- **App crashes**: Check `adb logcat` for errors
- **API timeout**: Increase timeout in `ApiService.cs`
- **Certificate errors**: Already handled in debug mode

### Getting Help
1. Check `adb logcat` for errors
2. Check API logs in console
3. Verify network connectivity
4. Test API with Postman/browser first
