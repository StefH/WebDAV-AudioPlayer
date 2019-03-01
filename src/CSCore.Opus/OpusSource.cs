using Concentus.Oggfile;
using Concentus.Structs;
using System;
using System.IO;

namespace CSCore.Opus
{
    /// <seealso cref="IWaveSource" />
    public sealed class OpusSource : IWaveSource
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private readonly Stream _stream;

        public OpusSource(Stream stream, int sampleRate, int channels)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("OpusStream is not readable.", nameof(stream));
            }

            _stream = stream;

            WaveFormat = new WaveFormat(sampleRate, 16, channels);
        }

        public void Decode()
        {
            var decoder = new OpusDecoder(WaveFormat.SampleRate, WaveFormat.Channels);
            var opus = new OpusOggReadStream(decoder, _stream);

            while (opus.HasNextPacket)
            {
                short[] packets = opus.DecodeNextPacket();
                if (packets != null && packets.Length > 0)
                {
                    byte[] buffer = new byte[packets.Length * 2];
                    Buffer.BlockCopy(packets, 0, buffer, 0, buffer.Length);
                    _memoryStream.Write(buffer, 0, buffer.Length);
                }
            }

            _memoryStream.Seek(0, SeekOrigin.Begin);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _memoryStream.Read(buffer, offset, count);
        }

        public WaveFormat WaveFormat { get; }

        public bool CanSeek => _memoryStream.CanSeek;

        public long Position
        {
            get => _memoryStream.Position;

            set
            {
                if (value < 0 || value > Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _memoryStream.Position = value;
            }
        }

        public long Length => _memoryStream.Length;

        public void Dispose()
        {
            _memoryStream.Dispose();
        }
    }
}