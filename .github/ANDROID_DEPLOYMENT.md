# GitHub Actions - Android Deployment Setup

## Overview
Automated Android build pipeline that:
- Builds signed APK and AAB files
- Auto-increments version on each build
- Uploads artifacts to GitHub
- Creates releases on main branch

## Initial Setup

### 1. Generate Keystore
Run the PowerShell script to create your signing keystore:
```powershell
.\generate-keystore.ps1
```

Follow the prompts to enter:
- Your name and organization details
- Keystore password (remember this!)
- Key password (remember this!)

### 2. Convert Keystore to Base64
After generating the keystore, convert it to Base64:
```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('schoolmanagement.keystore')) | Set-Clipboard
```

### 3. Add GitHub Secrets
Go to your GitHub repository: **Settings > Secrets and variables > Actions**

Add these secrets:
- `ANDROID_KEYSTORE_BASE64`: Paste the Base64 string from clipboard
- `ANDROID_KEY_ALIAS`: `schoolmanagement` (or your custom alias)
- `ANDROID_KEY_PASSWORD`: The key password you entered
- `ANDROID_KEYSTORE_PASSWORD`: The keystore password you entered

### 4. Store Keystore Safely
**IMPORTANT**: Keep `schoolmanagement.keystore` file in a secure location. You'll need the same keystore for all future app updates!

## Workflow Triggers

The workflow runs on:
- Push to `main` or `develop` branches
- Pull requests to `main` branch
- Manual trigger via GitHub Actions UI

## Versioning

Version format: `1.0.BUILD_NUMBER`
- `versionCode`: GitHub run number (auto-incremented)
- `versionName`: `1.0.BUILD_NUMBER`

Example: Build #42 → Version `1.0.42` (versionCode: 42)

## Build Outputs

### Artifacts (All Builds)
- **APK**: `android-apk-v1.0.X` - For direct installation
- **AAB**: `android-aab-v1.0.X` - For Google Play Store

Artifacts are kept for 30 days.

### Releases (Main Branch Only)
When pushing to `main`, a GitHub release is created with:
- Tag: `v1.0.X`
- Both APK and AAB files attached

## Manual Trigger

To manually trigger a build:
1. Go to **Actions** tab in GitHub
2. Select **Android Build and Deploy**
3. Click **Run workflow**
4. Choose branch and click **Run workflow**

## Troubleshooting

### Build Fails - Keystore Issues
- Verify all 4 secrets are set correctly
- Ensure Base64 string is complete (no truncation)
- Check alias matches the one used during keystore generation

### Version Conflicts
- Each build auto-increments based on GitHub run number
- No manual intervention needed

### Keystore Lost
If you lose the keystore file:
- You cannot update the existing app on Play Store
- You'll need to publish as a new app with a new package name
- **Always backup your keystore!**

## Local Testing

To test signed build locally:
```powershell
dotnet publish SchoolManagement/SchoolManagement.csproj `
  -f net9.0-android `
  -c Release `
  /p:AndroidSigningKeyStore=schoolmanagement.keystore `
  /p:AndroidSigningKeyAlias=schoolmanagement `
  /p:AndroidSigningKeyPass=YOUR_KEY_PASSWORD `
  /p:AndroidSigningStorePass=YOUR_KEYSTORE_PASSWORD
```

## Publishing to Google Play

1. Download AAB file from GitHub Actions artifacts or releases
2. Go to Google Play Console
3. Create new release or update existing app
4. Upload the AAB file
5. Complete release details and publish

## Security Notes

- Never commit keystore files to repository
- Never commit passwords or secrets
- Use GitHub Secrets for all sensitive data
- Keystore is decoded at build time and not persisted
