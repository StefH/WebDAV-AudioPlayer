using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiContrib.Core.Results;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Middlewares
{
    public class SoundFileProviderMiddleware
    {
        private readonly Regex RegexGuid = new Regex("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public SoundFileProviderMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestPath = context.Request.Path.ToString();

            if (requestPath.StartsWith(AudioPlayerConstants.SoundPrefix) && Guid.TryParse(RegexGuid.Match(requestPath).Value, out Guid id))
            {
                if (_cache.TryGetValue(id, out ResourceItem cachedResourceItem))
                {
                    cachedResourceItem.Stream.Position = 0;
                    await context.File(cachedResourceItem.Stream, MimeTypeMap.GetMimeType(cachedResourceItem.Extension), true);
                }
                else
                {
                    await context.NotFound();
                }
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }
    }
}