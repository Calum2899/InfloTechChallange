using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserManagement.Client.Models;

namespace UserManagement.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly JwtAuthStateProvider _auth;

    public ApiClient(HttpClient http, JwtAuthStateProvider auth)
    {
        _http = http; _auth = auth;
    }

    async Task EnsureAuthAsync()
    {
        var token = await _auth.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<UserDto>?> GetUsersAsync(bool? isActive)
    {
        await EnsureAuthAsync();
        var qs = isActive is null ? "" : $"?isActive={isActive.ToString()!.ToLower()}";
        return await _http.GetFromJsonAsync<List<UserDto>>($"api/users{qs}");
    }

    public async Task<UserDto?> GetUserAsync(long id)
    {
        await EnsureAuthAsync();
        return await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
    }

    public async Task<long?> CreateUserAsync(CreateUserDto dto)
    {
        await EnsureAuthAsync();
        var resp = await _http.PostAsJsonAsync("api/users", dto);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<long>();
    }

    public async Task UpdateUserAsync(long id, UpdateUserDto dto)
    {
        await EnsureAuthAsync();
        var resp = await _http.PutAsJsonAsync($"api/users/{id}", dto);
        resp.EnsureSuccessStatusCode();
    }
    public async Task DeleteUserAsync(long id, CancellationToken ct = default)
    {
        var resp = await _http.DeleteAsync($"api/users/{id}", ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", new LoginDto(email, password));
        if (!resp.IsSuccessStatusCode) return false;
        var result = await resp.Content.ReadFromJsonAsync<AuthResultDto>();
        if (result is null) return false;
        await _auth.SetTokenAsync(result.Token, result.ExpiresUtc);
        return true;
    }
}
