# STATIFY

## Description
Statify is a web-based Spotify analytics tool built with **Blazor WebAssembly (C#)**.  
The goal is to make your music taste visible, fun, and easy to explore.

Users can see their top artists, tracks, and genres, and compare any two Spotify timeframes with the **Music Match** feature.

This project was made for Project 3 as a working, polished prototype.  
All data comes directly from your real Spotify account using OAuth authorization.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourname/Statify.git
   cd Statify
   ```

2. Add your Spotify API keys  
   Create an app at: https://developer.spotify.com/dashboard  
   Then place your credentials in `appsettings.json` or environment variables.

3. Run the project:
   ```bash
   dotnet run
   ```
4. Open the browser at:
   ```
   https://localhost:5001
   ```

## Features

* **Spotify Login** (OAuth)
* **Dashboard** with:
  * Top Artists  
  * Top Tracks  
  * Top Genres  
  * Clean Spotify-inspired UI
* **Music Match**
  * Compare 4 weeks, 6 months, and all-time listening  
  * Shared artists  
  * Shared tracks  
  * Shared genres  
  * Match percentage (0–99%)
  * Artist & track navigation with arrow buttons  
* Fully responsive layout  
* All images & data from real Spotify API  
* Smooth UI with chips, cards, circles, and fade animations

## How It Works

1. The user logs in with Spotify.
2. Statify receives an access token via OAuth.
3. The app loads real data:
   * Top artists  
   * Top tracks  
   * Artist genres  
   for all three timeframes.
4. Dashboard displays everything visually.
5. Music Match compares two selected timeframes and generates:
   * Match score  
   * Similarity message  
   * Side-by-side artists  
   * Side-by-side tracks  
   * Genre overlaps  

Everything updates instantly without page reloads.

## Future Plans

* Save profiles to a database  
* Shareable Music Match links  
* Playlist generation (“Best of Both Timeframes”)  
* Social profiles  
* Public “Statify Wrapped” mode  

## Screenshots

![Landingpage Screenshot](/wwwroot/img/screenshot-landingpage.png)
![Machtigingen Screenshot](/wwwroot/img/screenshot-machtigingen.png)
![Dashboard Screenshot](/wwwroot/img/screenshot-dashboard.png)
![Music Match Screenshot](/wwwroot/img/screenshot-musicmatch.png)

## Author

**Shugri Farah**  
Class Oranje 2025  

