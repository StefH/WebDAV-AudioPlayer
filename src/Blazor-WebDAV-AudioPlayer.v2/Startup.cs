using System;
using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Middlewares;
using Blazor.WebDAV.AudioPlayer.Options;
using Howler.Blazor.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddMemoryCache(memoryCacheOptions =>
            {
                memoryCacheOptions.SizeLimit = 5;
            });

            services.AddSingleton<IConnectionSettings>((serviceProvider) =>
            {
                var section = Configuration.GetSection("ConnectionSettings");

                return new ConnectionSettings
                {
                    Password = section["Password"],
                    RootFolder = section["RootFolder"],
                    StorageUri = new Uri(section["StorageUri"]),
                    UserName = section["UserName"]
                };
            });

            // services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new ConnectionSettings()));

            services.AddSingleton<IWebDavClient, MyWebDavClient>();
            services.AddScoped<IHowl, Howl>();
            services.AddScoped<IPlayer, Player>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<SoundFileProviderMiddleware>();

            ConfigureStaticFiles(app);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void ConfigureStaticFiles(IApplicationBuilder app)
        {
            app.UseStaticFiles(); // For the wwwroot folder

            var provider = new FileExtensionContentTypeProvider();

            // Replace an existing mapping
            provider.Mappings[".ogg"] = "audio/ogg";

            // Add new mapping
            provider.Mappings[".opus"] = "audio/opus";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }
    }
}