using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Statify;
using Statify.Services; // ← waar je Spotify-services staan

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient voor API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// >>> HIER al je eigen services registreren <<<
// pas aan aan de namen die je echt hebt
builder.Services.AddScoped<SpotifyAuthService>();

// als je een MusicMatchService hebt:
// builder.Services.AddScoped<MusicMatchService>();

await builder.Build().RunAsync();
