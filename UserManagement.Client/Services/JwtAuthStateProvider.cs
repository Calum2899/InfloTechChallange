using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace UserManagement.Client.Services;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private const string Key = "jwt";
    private const string ExpKey = "jwt_exp";

    public JwtAuthStateProvider(IJSRuntime js) => _js = js;

    public async Task<string?> GetTokenAsync()
    {
        try { return await _js.InvokeAsync<string>("localStorage.getItem", Key); }
        catch { return null; }
    }

    public async Task SetTokenAsync(string token, DateTime expiresUtc)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", Key, token);
        await _js.InvokeVoidAsync("localStorage.setItem", ExpKey, expiresUtc.ToString("o"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", Key);
        await _js.InvokeVoidAsync("localStorage.removeItem", ExpKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", Key);
            var expIso = await _js.InvokeAsync<string>("localStorage.getItem", ExpKey);
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(expIso))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (!DateTime.TryParse(expIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expUtc)
                || expUtc <= DateTime.UtcNow)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
