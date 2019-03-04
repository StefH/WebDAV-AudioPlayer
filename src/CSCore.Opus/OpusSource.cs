using Concentus.Oggfile;
using Concentus.Structs;
using CSCore.Opus.Memory;
using System;
using System.Buffers;
using System.IO;
using System.Threading;

namespace CSCore.Codecs.OPUS
{
    /// <seealso cref="IWaveSource" />
    public sealed class OpusSource : IWaveSource
    {
        private const int BitsPerSample = 16;
        private const int BytesPerSample = BitsPerSample / 8;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly CircularByteQueue _circularByteQueue = new CircularByteQueue(1024 * 1024);
        private readonly ArrayPool<byte> _buffer = ArrayPool<byte>.Shared;
        private readonly Stream _stream;
        private readonly OpusOggReadStream _opusReadStream;

        private long _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpusSource"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="channels">The channels.</param>
        public OpusSource(Stream stream, int sampleRate, int channels)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable.", nameof(stream));
            }

            _stream = stream;

            WaveFormat = new WaveFormat(sampleRate, BitsPerSample, channels);

            var decoder = new OpusDecoder(WaveFormat.SampleRate, WaveFormat.Channels);
            _opusReadStream = new OpusOggReadStream(decoder, stream);

            _cts.Token.Register(() =>
            {
                decoder.ResetState();
            });
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int pos = 0;
            while (!_cts.IsCancellationRequested && _opusReadStream.HasNextPacket && pos < count)
            {
                short[] packets = _opusReadStream.DecodeNextPacket();
                if (packets != null && packets.Length > 0)
                {
                    int length = packets.Length * 2;
                    byte[] temp = _buffer.Rent(length);
                    try
                    {
                        Buffer.BlockCopy(packets, 0, temp, 0, length);

                        pos += temp.Length;

                        _circularByteQueue.Enqueue(temp, 0, length);
                    }
                    finally
                    {
                        _buffer.Return(temp);
                    }
                }
            }

            int bytesReadFromBuffer = _circularByteQueue.Dequeue(buffer, offset, count);

            _position += bytesReadFromBuffer;

            return bytesReadFromBuffer;
        }

        public WaveFormat WaveFormat { get; }

        public bool CanSeek => _stream.CanSeek;

        public long Position
        {
            get => _position;

            set
            {
                // Clear the queue to remove pending samples
                _circularByteQueue.Clear();

                // Jump to the new position in the stream
                _opusReadStream.SeekTo(Convert.ToInt64(value / (1.0 * WaveFormat.Channels * BytesPerSample)));

                // Update the position to the seek position
                _position = Convert.ToInt64(1.0 * _opusReadStream.PageGranulePosition * WaveFormat.Channels * BytesPerSample);
            }
        }

        public long Length => Convert.ToInt64(_opusReadStream.GranuleCount * WaveFormat.Channels * BytesPerSample);

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}