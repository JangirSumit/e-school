using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SchoolManagement.DTOs;

namespace SchoolManagement.Services;

public interface IApiService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<SignupResponse?> SignupAsync(SignupRequest request);
    Task<List<StudentResponse>> GetStudentsAsync();
    Task<StudentResponse?> CreateStudentAsync(CreateStudentRequest request);
    Task DeleteStudentAsync(string id);
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
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
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
            {
                return null;
            }
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
            {
                var error = await response.Content.ReadAsStringAsync();
                return new SignupResponse(false, $"Server error: {response.StatusCode}");
            }
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

    public async Task<List<StudentResponse>> GetStudentsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<StudentResponse>>("/api/students", _jsonOptions) ?? new();
    }

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
}
