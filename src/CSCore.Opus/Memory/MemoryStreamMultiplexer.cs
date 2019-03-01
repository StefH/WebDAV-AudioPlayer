using System;
using System.Collections.Generic;
using System.Threading;

namespace CSCore.Opus.Memory
{
    /// <summary>
    /// MultiThreaded buffer where one thread can write and many threads can read simultaneously. 
    /// </summary>
    public class MemoryStreamMultiplexer : IDisposable
    {
        private readonly ManualResetEvent[] _dataReadyEvents = new ManualResetEvent[255];
        private readonly ManualResetEvent[] _finishedEvents = new ManualResetEvent[255];
        private int _readerCount;
        private bool _finished;

        public int Length { get; private set; }

        public List<byte[]> Buffer { get; } = new List<byte[]>();

        public void Write(byte[] data, int pos, int length)
        {
            byte[] newBuf = new byte[length];
            System.Buffer.BlockCopy(data, pos, newBuf, 0, length);
            lock (Buffer)
            {
                Buffer.Add(newBuf);
                Length += length;
            }
            Set();
        }

        private void Set()
        {
            for (int i = 0; i < _readerCount; i++)
            {
                _dataReadyEvents[i].Set();
            }
        }

        public void Finish()
        {
            for (int i = 0; i < _readerCount; i++)
            {
                _finishedEvents[i].Set();
            }

            _finished = true;
        }

        public MemoryStreamReader GetReader()
        {
            var dataReady = new ManualResetEvent(_finished);
            var finished = new ManualResetEvent(_finished);

            lock (_dataReadyEvents)
            {
                _dataReadyEvents[_readerCount] = dataReady;
                _finishedEvents[_readerCount] = finished;
                _readerCount++;
            }

            return new MemoryStreamReader(Buffer, dataReady, finished);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Finish();

                for (int i = 0; i < _readerCount; i++)
                {
                    _dataReadyEvents[i].Dispose();
                    _finishedEvents[i].Dispose();
                }

                _readerCount = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryStreamMultiplexer()
        {
            Dispose(false);
        }
    }
}