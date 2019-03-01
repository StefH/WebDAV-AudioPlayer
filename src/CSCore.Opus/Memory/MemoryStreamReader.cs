using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CSCore.Opus.Memory
{
    public class MemoryStreamReader : Stream
    {
        private int _position;
        private int _bufferIndex;
        private int _bufferPos;
        private readonly List<byte[]> _bufferList;

        private readonly WaitHandle[] _waitHandles;
        private readonly ManualResetEvent _dataReady;

        public MemoryStreamReader(List<byte[]> bufferList, ManualResetEvent dataReady, ManualResetEvent finished)
        {
            _waitHandles = new WaitHandle[] { dataReady, finished };
            _bufferList = bufferList;
            _dataReady = dataReady;
            _bufferPos = 0;
            _bufferIndex = 0;
            _position = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_bufferIndex < _bufferList.Count)
            {
                return ReadInternal(buffer, offset, count);
            }

            _dataReady.Reset();

            // Wait for either data ready event of the finished event.
            WaitHandle.WaitAny(_waitHandles, TimeSpan.FromSeconds(30), false);

            // either of the event fired. see if there's more data to read.
            if (_bufferIndex < _bufferList.Count)
            {
                return ReadInternal(buffer, offset, count);
            }

            return 0; // No more bytes will be available. Finished.
        }

        private int ReadInternal(byte[] buffer, int offset, int count)
        {
            byte[] currentBuffer = _bufferList[_bufferIndex];

            if (_bufferPos + count <= currentBuffer.Length)
            {
                // the current buffer holds the same or more bytes than what is asked for
                // So, give what was asked.
                Buffer.BlockCopy(currentBuffer, _bufferPos, buffer, offset, count);

                _bufferPos += count;
                _position += count;
                return count;
            }

            // current buffer does not have the necessary bytes. deliver whatever is available.
            if (_bufferPos < currentBuffer.Length)
            {
                int remainingBytes = currentBuffer.Length - _bufferPos;
                Buffer.BlockCopy(currentBuffer, _bufferPos, buffer, offset, remainingBytes);

                _position += remainingBytes;
                _bufferIndex++;
                _bufferPos = 0;

                // Try to read from the next buffer in the list and deliver
                // the undelivered bytes. The Read call might block and wait for 
                // remaining bytes to appear. 
                return remainingBytes + Read(buffer, offset + remainingBytes, count - remainingBytes);
            }

            // Already all bytes from current buffer has been delivered. Try next buffer.
            _bufferIndex++;
            _bufferPos = 0;

            // There may not be next buffer and thus we will have to wait.
            return Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
