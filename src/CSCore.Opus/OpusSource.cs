using Concentus.Oggfile;
using Concentus.Structs;
using CSCore.Opus.Memory;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSCore.Opus
{
    /// <seealso cref="IWaveSource" />
    public sealed class OpusSource : IWaveSource
    {
        private readonly double _durationInMs;
        private readonly MemoryStreamMultiplexer _memoryStreamMultiplexer = new MemoryStreamMultiplexer();
        private readonly MemoryStreamReader _memoryStreamReader;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public OpusSource(Stream stream, int sampleRate, int channels, double durationInMs)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable.", nameof(stream));
            }

            _durationInMs = durationInMs;

            WaveFormat = new WaveFormat(sampleRate, 16, channels);

            _memoryStreamReader = _memoryStreamMultiplexer.GetReader();

            DecodeAsync(stream);
        }

        private void DecodeAsync(Stream stream)
        {
            var decoder = new OpusDecoder(WaveFormat.SampleRate, WaveFormat.Channels);
            var opus = new OpusOggReadStream(decoder, stream);

            Task.Run(() =>
            {
                while (opus.HasNextPacket && !_cts.IsCancellationRequested)
                {
                    short[] packets = opus.DecodeNextPacket();
                    if (packets != null && packets.Length > 0)
                    {
                        byte[] buffer = new byte[packets.Length * 2];
                        Buffer.BlockCopy(packets, 0, buffer, 0, buffer.Length);
                        _memoryStreamMultiplexer.Write(buffer, 0, buffer.Length);
                    }
                }
            }, _cts.Token);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _memoryStreamReader.Read(buffer, offset, count);
        }

        public WaveFormat WaveFormat { get; }

        public bool CanSeek => _memoryStreamReader.CanSeek;

        public long Position
        {
            get => _memoryStreamReader.Position;

            set
            {
                if (value < 0 || value > Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _memoryStreamReader.Position = value;
            }
        }

        public long Length => (long)(_durationInMs / 1000.0 * WaveFormat.SampleRate * WaveFormat.Channels * WaveFormat.BitsPerSample / 8);

        public void Dispose()
        {
            _memoryStreamMultiplexer?.Dispose();
            _memoryStreamReader?.Dispose();
            _cts?.Dispose();
        }
    }
}