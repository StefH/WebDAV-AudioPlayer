using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Options;
using Howler.Blazor.Components;
using Microsoft.AspNetCore.Builder;
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

            // builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMemoryCache(memoryCacheOptions =>
            {
                memoryCacheOptions.SizeLimit = 5;
            });
            builder.Services.AddSingleton<IWebDavClient, MyWebDavClient>();
            builder.Services.AddScoped<IHowl, Howl>();
            builder.Services.AddScoped<IHowlGlobal, HowlGlobal>();
            builder.Services.AddScoped<IPlayer, Player>();

            //builder.Services.AddSingleton<IConnectionSettings>(serviceProvider =>
            //{
            //    return new ConnectionSettings
            //    {
            //        Password = Secrets.Password,
            //        RootFolder = Secrets.RootFolder,
            //        StorageUri = new Uri(Secrets.StorageUri),
            //        UserName = Secrets.UserName
            //    };
            //});

            await builder.Build().RunAsync();
        }
    }
}