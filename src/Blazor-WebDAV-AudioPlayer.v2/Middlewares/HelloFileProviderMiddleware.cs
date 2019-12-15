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
        private readonly IConnectionSettings _settings;

        public HelloFileProviderMiddleware(RequestDelegate next, IWebDavClient client, IConnectionSettings settings)
        {
            _next = next;
            _client = client;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestPath = context.Request.Path.ToString();

            if (requestPath.Contains("_sounds_"))
            {
                string path = requestPath.Replace("/_sounds_", _settings.StorageUri.ToString());

                var fi = new FileInfo(path);

                var ri = new ResourceItem();
                ri.DisplayName = fi.Name;
                ri.FullPath = new Uri(path);

                await _client.GetStreamAsync(ri, CancellationToken.None);

                await context.File(ri.Stream, MimeTypeMap.GetMimeType(fi.Extension), true);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }
    }
}