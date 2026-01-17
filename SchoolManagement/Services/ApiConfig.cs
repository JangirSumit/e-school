namespace SchoolManagement.Services;

public static class ApiConfig
{
#if DEBUG
    #if ANDROID
        public const string BaseUrl = "http://10.0.2.2:7001"; // Android Emulator
    #elif WINDOWS
        public const string BaseUrl = "http://localhost:5001"; // Windows - HTTP port
    #else
        public const string BaseUrl = "https://localhost:7001"; // iOS/Mac
    #endif
#else
    public const string BaseUrl = "https://your-production-api.com";
#endif
}
