using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Client;
using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMemoryCache(memoryCacheOptions =>
            {
                memoryCacheOptions.SizeLimit = 5;
            });
            builder.Services.AddSingleton<IWebDavClient, ApiClient>();
            builder.Services.AddScoped<IHowl, Howl>();
            builder.Services.AddScoped<IHowlGlobal, HowlGlobal>();
            builder.Services.AddScoped<IPlayer, Player>();

            await builder.Build().RunAsync();
        }
    }
}