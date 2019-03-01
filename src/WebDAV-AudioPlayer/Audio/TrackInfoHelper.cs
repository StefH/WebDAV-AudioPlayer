﻿using ATL;
using System;
using System.IO;

namespace WebDav.AudioPlayer.Audio
{
    internal static class TrackInfoHelper
    {
        public static MediaDetails GetMediaDetails(Stream stream, string extension)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                var track = new Track(stream, extension);

                return new MediaDetails
                {
                    BitrateKbps = track.Bitrate,
                    Mode = track.IsVBR ? "VBR" : "CBR",
                    SampleRate = Convert.ToInt32(track.SampleRate),
                    Channels = 2,
                    DurationMs = track.DurationMs
                };
            }
            finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
