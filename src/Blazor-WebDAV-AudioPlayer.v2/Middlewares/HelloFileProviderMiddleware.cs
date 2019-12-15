using Blazor.WebDAV.AudioPlayer.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiContrib.Core.Results;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Middlewares
{
    public class HelloFileProviderMiddleware
    {
        private const string Prefix = "/sounds/";
        private readonly Regex GuidRegex = new Regex("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        private readonly RequestDelegate _next;
        private readonly IWebDavClient _client;
        private readonly IConnectionSettings _settings;
        private readonly IMemoryCache _cache;

        public HelloFileProviderMiddleware(RequestDelegate next, IWebDavClient client, IConnectionSettings settings, IMemoryCache cache)
        {
            _next = next;
            _client = client;
            _settings = settings;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestPath = context.Request.Path.ToString();

            if (requestPath.StartsWith(Prefix) && Guid.TryParse(GuidRegex.Match(requestPath).Value, out Guid id))
            {
                if (_cache.TryGetValue(id, out ResourceItem cachedResourceItem))
                {
                    await context.File(cachedResourceItem.Stream, MimeTypeMap.GetMimeType(cachedResourceItem.Extension), true);
                }

                //await _client.GetStreamAsync(ri, CancellationToken.None);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }
    }
}