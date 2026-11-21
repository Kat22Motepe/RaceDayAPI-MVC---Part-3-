using System.Net.Http.Json;
using RaceDay.API.Dtos;
using RaceDay.Domain;

namespace RaceDay.API.Tests;

public static class Helpers
{
    // Registers a user and logs them in, returning their token
    public static async Task<LoginResponse> RegisterAndLogin(
     HttpClient client, string email, string password, UserRole role)
    {
        // 1. Register the user
        var regRes = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
            "Test", "User", email, password, role));

        // 2. Read the error if it failed (this helps us debug!)
        if (!regRes.IsSuccessStatusCode)
        {
            var error = await regRes.Content.ReadAsStringAsync();
            throw new Exception($"Registration failed during test setup. Status: {regRes.StatusCode}. Error: {error}");
        }

        // 3. Log them in
        var loginRes = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, password));

        return (await loginRes.Content.ReadFromJsonAsync<LoginResponse>())!;
    }

    // Attaches the token to the request headers
    public static void UseToken(this HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
}
