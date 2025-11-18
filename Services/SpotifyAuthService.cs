using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Statify.Services
{
    public class SpotifyAuthService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public SpotifyAuthService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        // Authorization code (eenmalig)
        public string? AuthCode { get; private set; }

        // Access token (hier gaan we straks mee naar de API)
        public string? AccessToken { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);

        public void StoreAuthCode(string code)
        {
            AuthCode = code;
        }

        public async Task<bool> ExchangeCodeForTokenAsync()
        {
            if (string.IsNullOrEmpty(AuthCode))
                return false;

            var clientId     = _config["Spotify:ClientId"];
            var clientSecret = _config["Spotify:ClientSecret"];
            var redirectUri  = _config["Spotify:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(clientSecret) ||
                string.IsNullOrEmpty(redirectUri))
            {
                return false;
            }

            // Basic auth header: base64(clientId:clientSecret)
            var authBytes  = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var authHeader = Convert.ToBase64String(authBytes);

            var form = new Dictionary<string, string>
            {
                ["grant_type"]   = "authorization_code",
                ["code"]         = AuthCode!,
                ["redirect_uri"] = redirectUri
            };

            using var req = new HttpRequestMessage(
                HttpMethod.Post,
                "https://accounts.spotify.com/api/token");

            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            req.Content = new FormUrlEncodedContent(form);

            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return false;

            var json = await resp.Content.ReadAsStringAsync();

            var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(json);
            AccessToken = token?.access_token;

            return !string.IsNullOrEmpty(AccessToken);
        }

        // Kleine DTO voor het antwoord van Spotify
        private class SpotifyTokenResponse
        {
            public string? access_token  { get; set; }
            public string? token_type    { get; set; }
            public int     expires_in    { get; set; }
            public string? refresh_token { get; set; }
            public string? scope         { get; set; }
        }
    }
}
