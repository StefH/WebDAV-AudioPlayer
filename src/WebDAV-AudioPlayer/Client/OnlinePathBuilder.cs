using System;

namespace WebDav.AudioPlayer.Client
{
    internal static class OnlinePathBuilder
    {
        /// <summary>
        /// Converts a local path to an URL on for your WebDAV account.
        /// </summary>
        /// <param name="host">The URL to the stack account.</param>
        /// <param name="location">The local path. E.g. /MyFolder/MyFile.zip</param>
        /// <returns>URI to the location on the WebDAV.</returns>
        public static Uri ConvertPathToFullUri(Uri host, string location = null)
        {
            if (location == null)
                return new Uri(SanitizeHost(host), UriKind.Absolute);

            return new Uri(SanitizeHost(host) + location.TrimStart('/'), UriKind.Absolute);
        }

        private static string SanitizeHost(Uri host)
        {
            string left = host.GetLeftPart(UriPartial.Path);
            return left.EndsWith("/") ? left : left + "/";
        }

        public static Uri Combine(Uri host, string path)
        {
            return new Uri(host, path);
        }

        //public static Uri Combine(Uri host, Uri path)
        //{
        //    return new Uri(host, path);
        //}
    }
}