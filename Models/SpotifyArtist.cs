namespace Statify.Models
{
    public class SpotifyArtist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public List<string> Genres { get; set; } = new();
        public List<SpotifyImage> Images { get; set; } = new();
    }
}
