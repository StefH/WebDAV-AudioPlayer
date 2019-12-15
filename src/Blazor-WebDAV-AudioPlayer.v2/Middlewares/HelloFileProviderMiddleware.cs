using Blazor.WebDAV.AudioPlayer.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiContrib.Core.Results;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Middlewares
{
    public class HelloFileProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebDavClient _client;

        public HelloFileProviderMiddleware(RequestDelegate next, IWebDavClient client)
        {
            _next = next;
            _client = client;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains("sheyenrath"))
            {
                var ri = new ResourceItem();
                ri.DisplayName = "sheyenrath.mp3";
                ri.FullPath = new Uri("https://sheyenrath.stackstorage.com/remote.php/webdav/mp3-sync/Chillout/Amethystium/Amethystium%20-%20Half%20a%20World%20Away%20%28with%20Caroline%20Lavelle%29.mp3");

                await _client.GetStreamAsync(ri, CancellationToken.None);

                await context.File(ri.Stream, MimeTypeMap.GetMimeType(".mp3"), true);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }
    }
}