using System;
using System.Collections.Concurrent;
using System.Linq;

namespace WebDav.AudioPlayer.Util
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly Action<T, int> _dequeueFunc;
        private readonly object _syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size, Action<T, int> dequeueFunc = null)
        {
            _dequeueFunc = dequeueFunc;
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            if (this.Contains(obj))
                return;

            base.Enqueue(obj);

            Clear(Size);
        }

        public new void Clear()
        {
            Clear(0);
        }

        private void Clear(int sizeToKeep)
        {
            lock (_syncObject)
            {
                while (Count > sizeToKeep)
                {
                    T outObj;
                    if (TryDequeue(out outObj))
                        if (_dequeueFunc != null)
                            _dequeueFunc.Invoke(outObj, Count);
                }
            }
        }
    }
}