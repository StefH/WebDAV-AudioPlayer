using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Client;
using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
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

            // HttpClient
            var baseAddress = builder.HostEnvironment.BaseAddress;
            Console.WriteLine("baseAddress = " + baseAddress);

            bool isLocalHost = baseAddress.Contains("localhost");
            Console.WriteLine("isLocalHost = " + isLocalHost);

            bool isAzure = baseAddress.Contains("azurestaticapps.net") || baseAddress.Contains("music.heyenrath.nl");
            Console.WriteLine("isAzure = " + isAzure);

            string httpClientBaseAddress = isLocalHost ? "http://localhost:7071" : baseAddress;
            Console.WriteLine("httpClientBaseAddress = " + httpClientBaseAddress);
            // builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(httpClientBaseAddress) });

            // Own services
            builder.Services.AddScoped(sp =>
                 {
                     var httpClient = new HttpClient
                     {
                         BaseAddress = new Uri(httpClientBaseAddress)
                     };
                     return new RestClient(httpClient).For<IWebDAVFunctionApi>();
                 });


            builder.Services.AddMemoryCache(memoryCacheOptions =>
            {
                memoryCacheOptions.SizeLimit = 5;
            });
        //    builder.Services.AddSingleton<IWebDavClient, ApiClient>();
            builder.Services.AddScoped<IHowl, Howl>();
            builder.Services.AddScoped<IHowlGlobal, HowlGlobal>();
            builder.Services.AddScoped<IPlayer, Player>();

            await builder.Build().RunAsync();
        }
    }
}