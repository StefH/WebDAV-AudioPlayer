using System;
using System.IO;
using System.Runtime.InteropServices;
using MI = MediaInfo;
using Status = MediaInfo.Status;

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

        public static MediaDetails GetMediaDetails(Stream stream)
        {
            var mediaDetails = new MediaDetails();

            try
            {
                stream.Seek(0, SeekOrigin.Begin);

                using (var info = new MI.MediaInfo())
                {
                    string version = info.Option("Info_Version");

                    byte[] buffer = new byte[64 * 1024];
                    int bufferSize; //The size of the read file buffer

                    //Preparing to fill MediaInfo with a buffer
                    info.Open_Buffer_Init(stream.Length, 0);

                    //The parsing loop
                    do
                    {
                        //Reading data somewhere, do what you want for this.
                        bufferSize = stream.Read(buffer, 0, 64 * 1024);

                        //Sending the buffer to MediaInfo
                        GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        IntPtr fromBufferIntPtr = gc.AddrOfPinnedObject();
                        Status result = (Status)info.Open_Buffer_Continue(fromBufferIntPtr, (IntPtr)bufferSize);
                        gc.Free();

                        if ((result & Status.Finalized) == Status.Finalized)
                            break;

                        //Testing if MediaInfo request to go elsewhere
                        if (info.Open_Buffer_Continue_GoTo_Get() != -1)
                        {
                            long position = stream.Seek(info.Open_Buffer_Continue_GoTo_Get(), SeekOrigin.Begin); //Position the file
                            info.Open_Buffer_Init(stream.Length, position); //Informing MediaInfo we have performed the seek
                        }
                    } while (bufferSize > 0);

                    //Finalizing
                    info.Open_Buffer_Finalize(); //This is the end of the stream, MediaInfo must finish some work

                    mediaDetails.Format = info.Get(MI.StreamKind.Audio, 0, "Format");
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
