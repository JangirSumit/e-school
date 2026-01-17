namespace SchoolManagement.Services;

public static class ApiConfig
{
#if DEBUG
    #if ANDROID
        public const string BaseUrl = "http://10.0.2.2:7001"; // Android Emulator
        // For real device with USB debugging, use: adb reverse tcp:7001 tcp:7001
        // Then use: http://localhost:7001
    #else
        public const string BaseUrl = "https://localhost:7001"; // Windows/iOS/Mac
    #endif
#else
    public const string BaseUrl = "https://your-production-api.com";
#endif
}
