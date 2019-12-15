using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.FileProviders
{
    public class MyFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            // https://sheyenrath.stackstorage.com/remote.php/webdav/mp3-sync/Chillout/Amethystium/Amethystium%20-%20Half%20a%20World%20Away%20%28with%20Caroline%20Lavelle%29.mp3
            
            var ri = new ResourceItem();
            ri.FullPath = new Uri("https://sheyenrath.stackstorage.com/remote.php/webdav/mp3-sync/Chillout/Amethystium/Amethystium%20-%20Half%20a%20World%20Away%20%28with%20Caroline%20Lavelle%29.mp3");

            throw new NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}