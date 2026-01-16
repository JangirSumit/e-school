# Generate Android Keystore for Signing
# Run this script once to create your keystore file

$keystoreName = "schoolmanagement.keystore"
$alias = "schoolmanagement"
$validity = 10000

Write-Host "Generating Android Keystore..." -ForegroundColor Green
Write-Host ""

keytool -genkey -v `
    -keystore $keystoreName `
    -alias $alias `
    -keyalg RSA `
    -keysize 2048 `
    -validity $validity `
    -storetype PKCS12

Write-Host ""
Write-Host "Keystore generated successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Convert keystore to Base64:"
Write-Host "   [Convert]::ToBase64String([IO.File]::ReadAllBytes('$keystoreName')) | Set-Clipboard"
Write-Host ""
Write-Host "2. Add these secrets to your GitHub repository (Settings > Secrets and variables > Actions):"
Write-Host "   - ANDROID_KEYSTORE_BASE64: (paste from clipboard)"
Write-Host "   - ANDROID_KEY_ALIAS: $alias"
Write-Host "   - ANDROID_KEY_PASSWORD: (password you entered for key)"
Write-Host "   - ANDROID_KEYSTORE_PASSWORD: (password you entered for keystore)"
Write-Host ""
Write-Host "3. Keep $keystoreName file safe - you'll need it for future releases!" -ForegroundColor Red
