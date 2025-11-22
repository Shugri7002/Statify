namespace Statify
{
    public class SpotifyTokenStore
    {
        public string? AccessToken { get; set; }
        public bool HasToken => !string.IsNullOrEmpty(AccessToken);
    }
}
