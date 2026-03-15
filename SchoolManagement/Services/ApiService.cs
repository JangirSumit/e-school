using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SchoolManagement.DTOs;

namespace SchoolManagement.Services;

public interface IApiService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<SignupResponse?> SignupAsync(SignupRequest request);
    Task<DashboardResponse?> GetDashboardAsync();
    Task<List<StudentResponse>> GetStudentsAsync();
    Task<StudentResponse?> CreateStudentAsync(CreateStudentRequest request);
    Task DeleteStudentAsync(string id);
    Task<List<TenantSummaryResponse>> GetSchoolsAsync();
    Task<TenantSummaryResponse?> CreateSchoolAsync(CreateSchoolRequest request);
    Task UpdateSchoolStatusAsync(string id, bool isActive);
    Task<List<ClassResponse>> GetClassesAsync();
    Task<ClassResponse?> CreateClassAsync(CreateClassRequest request);
    Task<List<FacultyResponse>> GetFacultyAsync();
    Task<FacultyResponse?> CreateFacultyAsync(CreateFacultyRequest request);
    void SetAuthToken(string token);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

#if ANDROID
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
#else
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl)
        };
#endif

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Connection failed: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            throw new Exception("Request timeout. Please check if API is running.");
        }
    }

    public async Task<SignupResponse?> SignupAsync(SignupRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/signup", request);
            if (!response.IsSuccessStatusCode)
                return new SignupResponse(false, $"Server error: {response.StatusCode}");

            return await response.Content.ReadFromJsonAsync<SignupResponse>(_jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            return new SignupResponse(false, $"Connection failed: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return new SignupResponse(false, "Request timeout. Please check if API is running.");
        }
    }

    public Task<DashboardResponse?> GetDashboardAsync() =>
        _httpClient.GetFromJsonAsync<DashboardResponse>("/api/management/dashboard", _jsonOptions);

    public Task<List<StudentResponse>> GetStudentsAsync() =>
        _httpClient.GetFromJsonAsync<List<StudentResponse>>("/api/students", _jsonOptions)!;

    public async Task<StudentResponse?> CreateStudentAsync(CreateStudentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/students", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StudentResponse>(_jsonOptions);
    }

    public async Task DeleteStudentAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"/api/students/{id}");
        response.EnsureSuccessStatusCode();
    }

    public Task<List<TenantSummaryResponse>> GetSchoolsAsync() =>
        _httpClient.GetFromJsonAsync<List<TenantSummaryResponse>>("/api/management/schools", _jsonOptions)!;

    public async Task<TenantSummaryResponse?> CreateSchoolAsync(CreateSchoolRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/management/schools", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TenantSummaryResponse>(_jsonOptions);
    }

    public async Task UpdateSchoolStatusAsync(string id, bool isActive)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/management/schools/{id}/status", new UpdateSchoolStatusRequest(isActive));
        response.EnsureSuccessStatusCode();
    }

    public Task<List<ClassResponse>> GetClassesAsync() =>
        _httpClient.GetFromJsonAsync<List<ClassResponse>>("/api/management/classes", _jsonOptions)!;

    public async Task<ClassResponse?> CreateClassAsync(CreateClassRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/management/classes", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClassResponse>(_jsonOptions);
    }

    public Task<List<FacultyResponse>> GetFacultyAsync() =>
        _httpClient.GetFromJsonAsync<List<FacultyResponse>>("/api/faculties", _jsonOptions)!;

    public async Task<FacultyResponse?> CreateFacultyAsync(CreateFacultyRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/faculties", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FacultyResponse>(_jsonOptions);
    }
}
