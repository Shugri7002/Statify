namespace Statify.Models
{
    // /v1/me
    public class SpotifyUserProfileResponse
    {
        public string? Id { get; set; }
        public string? Display_name { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? Product { get; set; } // free / premium
        public List<SpotifyImage>? Images { get; set; }
        public SpotifyFollowers? Followers { get; set; }
    }

    public class SpotifyFollowers
    {
        public int? Total { get; set; }
    }


    // /v1/me/top/artists
    public class SpotifyTopArtistsResponse
    {
        public List<SpotifyArtistItem> Items { get; set; } = new();
    }

    public class SpotifyArtistItem
    {
        public string? Name { get; set; }
        public List<SpotifyImage>? Images { get; set; }

        // Genres komen ook mee uit de API
        public List<string>? Genres { get; set; }
    }

    // /v1/me/top/tracks
    public class SpotifyTopTracksResponse
    {
        public List<SpotifyTrackItem> Items { get; set; } = new();
    }

    public class SpotifyTrackItem
    {
        public string? Name { get; set; }

        public List<SpotifyArtistItem>? Artists { get; set; }

        public SpotifyAlbum? Album { get; set; }
    }

    public class SpotifyAlbum
    {
        public string? Name { get; set; }
        public List<SpotifyImage>? Images { get; set; }
    }
}
