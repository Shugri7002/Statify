public class SpotifyTokenStore
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public bool IsValid =>
        !string.IsNullOrEmpty(AccessToken) &&
        ExpiresAt.HasValue &&
        ExpiresAt > DateTimeOffset.UtcNow;
}
