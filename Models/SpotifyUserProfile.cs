namespace Statify.Models
{
    public class SpotifyUserProfile
    {
        public string? DisplayName { get; set; }
        public List<SpotifyImage> Images { get; set; } = new();
    }

    public class SpotifyImage
    {
        public string? Url { get; set; }
    }
}
