using ATL;
using ATL.AudioData;
using System;
using System.IO;

namespace WebDav.AudioPlayer.Audio;

internal static class TrackInfoHelper
{
    public static MediaDetails GetMediaDetails(Stream stream, string mimeType)
    {
        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            var track = new Track(stream, mimeType);

            return new MediaDetails
            {
                Album = track.Album,
                Artist = track.Artist,
                Title = track.Title,
                BitrateKbps = track.Bitrate,
                Mode = track.IsVBR ? "VBR" : "CBR",
                SampleRate = Convert.ToInt32(track.SampleRate),
                Channels = track.ChannelsArrangement.NbChannels,
                DurationMs = track.DurationMs,
                IsLossless = track.CodecFamily == AudioDataIOFactory.CF_LOSSLESS
            };
        }
        finally
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
