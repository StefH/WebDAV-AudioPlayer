using System;
using System.IO;
using System.Runtime.InteropServices;
using MI = MediaInfo.DotNetWrapper;

namespace WebDav.AudioPlayer.Audio
{
    public static class MediaInfoHelper
    {
        private static readonly Func<MI.MediaInfo, string, int?> GetNullableIntValue = (info, parameter) =>
        {
            int result;
            if (int.TryParse(info.Get(MI.StreamKind.Audio, 0, parameter), out result))
                return result;

            return null;
        };

        public static bool TryGetVersion(out string version)
        {
            try
            {
                using (var info = new MI.MediaInfo())
                {
                    version = info.Option("Info_Version");
                    return true;
                }
            }
            catch (Exception)
            {
                version = null;
                return false;
            }
        }

        public static MediaDetails GetMediaDetails(Stream stream)
        {
            var mediaDetails = new MediaDetails();

            try
            {
                stream.Seek(0, SeekOrigin.Begin);

                using (var info = new MI.MediaInfo())
                {
                    byte[] buffer = new byte[64 * 1024];
                    int bufferSize; //The size of the read file buffer

                    //Preparing to fill MediaInfo with a buffer
                    info.OpenBufferInit(stream.Length, 0);

                    //The parsing loop
                    do
                    {
                        //Reading data somewhere, do what you want for this.
                        bufferSize = stream.Read(buffer, 0, 64 * 1024);

                        //Sending the buffer to MediaInfo
                        GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        IntPtr fromBufferIntPtr = gc.AddrOfPinnedObject();
                        MI.Status result = info.OpenBufferContinue(fromBufferIntPtr, (IntPtr)bufferSize);
                        gc.Free();

                        if ((result & MI.Status.Finalized) == MI.Status.Finalized)
                            break;

                        //Testing if MediaInfo request to go elsewhere
                        if (info.OpenBufferContinueGoToGet() != -1)
                        {
                            long position = stream.Seek(info.OpenBufferContinueGoToGet(), SeekOrigin.Begin); //Position the file
                            info.OpenBufferInit(stream.Length, position); //Informing MediaInfo we have performed the seek
                        }
                    } while (bufferSize > 0);

                    //Finalizing
                    info.OpenBufferFinalize(); //This is the end of the stream, MediaInfo must finish some work

                    string mode = info.Get(MI.StreamKind.Audio, 0, "BitRate_Mode");
                    mediaDetails.Mode = string.IsNullOrEmpty(mode) ? "CBR" : mode;
                    mediaDetails.Bitrate = GetNullableIntValue(info, "BitRate");
                    mediaDetails.Channels = GetNullableIntValue(info, "Channels");
                }
            }
            finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return mediaDetails;
        }
    }
}