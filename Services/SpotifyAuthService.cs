using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Statify.Models;

namespace Statify.Services
{
    public class SpotifyAuthService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public SpotifyAuthService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        // Authorization code (eenmalig)
        public string? AuthCode { get; private set; }

        // Access token (hier gaan we mee naar de API)
        public string? AccessToken { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);

        public void StoreAuthCode(string code)
        {
            AuthCode = code;
        }

        /// <summary>
        /// Wisselt de authorization code om voor een access token.
        /// Wordt aangeroepen in je AuthCallback.
        /// </summary>
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

            var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(json, _jsonOptions);
            AccessToken = token?.access_token;

            return !string.IsNullOrEmpty(AccessToken);
        }

        // ---------------- HELPER ----------------

        private string GetAccessTokenOrThrow()
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
                throw new InvalidOperationException("Geen Spotify access token gevonden. Gebruiker moet eerst inloggen.");

            return AccessToken;
        }

        private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url)
        {
            var token = GetAccessTokenOrThrow();

            var req = new HttpRequestMessage(method, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return req;
        }

        // ---------------- PROFIEL ----------------

        public async Task<SimpleProfile> GetProfileAsync()
        {
            using var req = CreateAuthorizedRequest(HttpMethod.Get, "https://api.spotify.com/v1/me");
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SpotifyUserProfileResponse>(json, _jsonOptions);

            var displayName = data?.Display_name ?? "Spotify User";
            var imageUrl    = data?.Images?.FirstOrDefault()?.Url;
            var email       = data?.Email;
            var country     = data?.Country;
            var product     = data?.Product;
            var followers   = data?.Followers?.Total;

            return new SimpleProfile(displayName, imageUrl, email, country, followers, product);
        }

        // Voor dashboard (naam + image)
        public async Task<(string? DisplayName, string? ImageUrl)> GetUserProfileAsync()
        {
            var p = await GetProfileAsync();
            return (p.DisplayName, p.ImageUrl);
        }

        // ---------------- TOP ARTISTS ----------------
        // timeRange: "short_term", "medium_term", "long_term"
        public async Task<List<SimpleArtist>> GetTopArtistsAsync(int limit = 7, string timeRange = "medium_term")
        {
            var url = $"https://api.spotify.com/v1/me/top/artists?limit={limit}&time_range={timeRange}";

            using var req = CreateAuthorizedRequest(HttpMethod.Get, url);
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SpotifyTopArtistsResponse>(json, _jsonOptions);

            var result = new List<SimpleArtist>();

            if (data?.Items != null)
            {
                foreach (var artist in data.Items)
                {
                    if (artist == null) continue;

                    var name   = artist.Name ?? "Unknown artist";
                    var img    = artist.Images?.FirstOrDefault()?.Url ?? "https://via.placeholder.com/160";
                    var genres = artist.Genres ?? new List<string>();

                    result.Add(new SimpleArtist(name, img, genres));
                }
            }

            return result;
        }

        // ---------------- TOP TRACKS ----------------

        public async Task<List<SimpleTrack>> GetTopTracksAsync(int limit = 7, string timeRange = "medium_term")
        {
            var url = $"https://api.spotify.com/v1/me/top/tracks?limit={limit}&time_range={timeRange}";

            using var req = CreateAuthorizedRequest(HttpMethod.Get, url);
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SpotifyTopTracksResponse>(json, _jsonOptions);

            var result = new List<SimpleTrack>();

            if (data?.Items != null)
            {
                foreach (var track in data.Items)
                {
                    if (track == null) continue;

                    var title      = track.Name ?? "Unknown track";
                    var artistName = track.Artists?.FirstOrDefault()?.Name ?? "Unknown artist";
                    var img        = track.Album?.Images?.FirstOrDefault()?.Url ?? "https://via.placeholder.com/160";

                    result.Add(new SimpleTrack(title, artistName, img));
                }
            }

            return result;
        }

        // ---------------- TOKEN-DTO ----------------

        private class SpotifyTokenResponse
        {
            public string? access_token  { get; set; }
            public string? token_type    { get; set; }
            public int     expires_in    { get; set; }
            public string? refresh_token { get; set; }
            public string? scope         { get; set; }
        }
    }

    // Eenvoudige modellen voor je UI
    public record SimpleProfile(
        string DisplayName,
        string? ImageUrl,
        string? Email,
        string? Country,
        int? Followers,
        string? Product
    );

    public record SimpleArtist(string Name, string ImageUrl, IReadOnlyList<string> Genres);
    public record SimpleTrack(string Title, string Artist, string ImageUrl);
}
