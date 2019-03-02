using Concentus.Oggfile;
using Concentus.Structs;
using CSCore.Opus.Memory;
using System;
using System.IO;
using System.Threading;

namespace CSCore.Opus
{
    /// <seealso cref="IWaveSource" />
    public sealed class OpusSource : IWaveSource
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly CircularByteQueue _circularByteQueue = new CircularByteQueue(1024 * 1024);

        private readonly double _durationInMs;
        private readonly OpusOggReadStream _opusReadStream;
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpusSource"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="durationInMs">The duration in ms.</param>
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

            var decoder = new OpusDecoder(WaveFormat.SampleRate, WaveFormat.Channels);
            _opusReadStream = new OpusOggReadStream(decoder, stream);

            _cts.Token.Register(() =>
            {
                decoder.ResetState();
            });

            //Task.Run(() =>
            //{
            //    while (!_cts.IsCancellationRequested && opusReadStream.HasNextPacket)
            //    {
            //        short[] packets = opusReadStream.DecodeNextPacket();
            //        if (packets != null && packets.Length > 0)
            //        {
            //            byte[] temp = new byte[packets.Length * 2];
            //            Buffer.BlockCopy(packets, 0, temp, 0, temp.Length);

            //            _circularByteQueue.Enqueue(temp, 0, temp.Length);
            //        }
            //    }
            //}, _cts.Token);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int pos = 0;
            while (!_cts.IsCancellationRequested && _opusReadStream.HasNextPacket & pos < count)
            {
                short[] packets = _opusReadStream.DecodeNextPacket();
                if (packets != null && packets.Length > 0)
                {
                    byte[] temp = new byte[packets.Length * 2];
                    Buffer.BlockCopy(packets, 0, temp, 0, temp.Length);

                    pos += temp.Length;

                    _circularByteQueue.Enqueue(temp, 0, temp.Length);
                }
            }

            int bytesReadFromBuffer = _circularByteQueue.Dequeue(buffer, offset, count);

            _position += bytesReadFromBuffer;

            return bytesReadFromBuffer;
        }

        public WaveFormat WaveFormat { get; }

        public bool CanSeek => false;

        public long Position
        {
            get => _position;

            set => throw new InvalidOperationException("Cannot set position on OpusSource.");
        }

        public long Length => (long)(_durationInMs / 1000.0 * WaveFormat.SampleRate * WaveFormat.Channels * WaveFormat.BitsPerSample / 8);

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}