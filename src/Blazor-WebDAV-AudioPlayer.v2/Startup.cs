using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Client;
using Blazor.WebDAV.AudioPlayer.Components;
using Blazor.WebDAV.AudioPlayer.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebDav.AudioPlayer.Audio;

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
            services.Configure<ConnectionSettings>(Configuration.GetSection("ConnectionSettings"));

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<IWebDavClientFactory, WebDavClientFactory>();
            services.AddScoped<IHowl, Howl>();
            services.AddScoped<IPlayerFactory, PlayerFactory>();
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

            app.UseStaticFiles(); // For the wwwroot folder

            ConfigureContentTypes(app);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void ConfigureContentTypes(IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();

            // Replace an existing mapping
            provider.Mappings[".ogg"] = "audio/ogg";

            // Add new mapping
            provider.Mappings[".opus"] = "audio/opus";

            app.UseStaticFiles(); // For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
            });
        }
    }
}