#region Copyright & License
/*************************************************************************
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014 Roman Atachiants (kelindar@gmail.com)

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*************************************************************************/
#endregion

using JetBrains.Annotations;
using System;

namespace CSCore.Opus.Memory
{
    /// <summary>
    /// Defines a class that represents a resizable circular byte queue.
    /// </summary>
    public sealed class CircularByteQueue
    {
        private readonly object _lockObject = new object();
        private readonly int _size;

        private int _head;
        private int _fail;
        private int _sizeUntilCut;
        private byte[] _internalBuffer;

        /// <summary>
        /// Gets the length of the byte queue
        /// </summary>
        [PublicAPI]
        public int Length { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularByteQueue"/> class.
        /// </summary>
        /// <param name="size">The buffer size.</param>
        public CircularByteQueue(int size)
        {
            _size = size;
            _internalBuffer = new byte[size];
        }

        /// <summary>
        /// Clears the byte queue
        /// </summary>
        public void Clear()
        {
            _head = 0;
            _fail = 0;
            Length = 0;
            _sizeUntilCut = _internalBuffer.Length;
        }

        /// <summary>
        /// Clears the byte queue
        /// </summary>
        public void Clear(int size)
        {
            lock (_lockObject)
            {
                if (size > Length)
                {
                    size = Length;
                }

                if (size == 0)
                {
                    return;
                }

                _head = (_head + size) % _internalBuffer.Length;
                Length -= size;

                if (Length == 0)
                {
                    _head = 0;
                    _fail = 0;
                }

                _sizeUntilCut = _internalBuffer.Length - _head;
            }
        }

        /// <summary>
        /// Extends the capacity of the ByteQueue
        /// </summary>
        private void SetCapacity(int capacity)
        {
            byte[] newBuffer = new byte[capacity];

            if (Length > 0)
            {
                if (_head < _fail)
                {
                    Buffer.BlockCopy(_internalBuffer, _head, newBuffer, 0, Length);
                }
                else
                {
                    Buffer.BlockCopy(_internalBuffer, _head, newBuffer, 0, _internalBuffer.Length - _head);
                    Buffer.BlockCopy(_internalBuffer, 0, newBuffer, _internalBuffer.Length - _head, _fail);
                }
            }

            _head = 0;
            _fail = Length;
            _internalBuffer = newBuffer;
        }

        /// <summary>
        /// Enqueues a buffer to the queue and inserts it to a correct position
        /// </summary>
        /// <param name="buffer">Buffer to enqueue</param>
        /// <param name="offset">The zero-based byte offset in the buffer</param>
        /// <param name="size">The number of bytes to enqueue</param>
        [PublicAPI]
        public void Enqueue(byte[] buffer, int offset, int size)
        {
            if (size == 0)
            {
                return;
            }

            lock (_lockObject)
            {
                if (Length + size > _internalBuffer.Length)
                {
                    SetCapacity((Length + size + _size - 1) & ~(_size - 1));
                }

                if (_head < _fail)
                {
                    int rightLength = _internalBuffer.Length - _fail;

                    if (rightLength >= size)
                    {
                        Buffer.BlockCopy(buffer, offset, _internalBuffer, _fail, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset, _internalBuffer, _fail, rightLength);
                        Buffer.BlockCopy(buffer, offset + rightLength, _internalBuffer, 0, size - rightLength);
                    }
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, _internalBuffer, _fail, size);
                }

                _fail = (_fail + size) % _internalBuffer.Length;
                Length += size;
                _sizeUntilCut = _internalBuffer.Length - _head;
            }
        }

        /// <summary>
        /// Dequeues a buffer from the queue
        /// </summary>
        /// <param name="buffer">Buffer to enqueue</param>
        /// <param name="offset">The zero-based byte offset in the buffer</param>
        /// <param name="size">The number of bytes to dequeue</param>
        /// <returns>Number of bytes dequeued</returns>
        [PublicAPI]
        public int Dequeue(byte[] buffer, int offset, int size)
        {
            lock (_lockObject)
            {
                if (size > Length)
                {
                    size = Length;
                }

                if (size == 0)
                {
                    return 0;
                }

                if (_head < _fail)
                {
                    Buffer.BlockCopy(_internalBuffer, _head, buffer, offset, size);
                }
                else
                {
                    int rightLength = _internalBuffer.Length - _head;

                    if (rightLength >= size)
                    {
                        Buffer.BlockCopy(_internalBuffer, _head, buffer, offset, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(_internalBuffer, _head, buffer, offset, rightLength);
                        Buffer.BlockCopy(_internalBuffer, 0, buffer, offset + rightLength, size - rightLength);
                    }
                }

                _head = (_head + size) % _internalBuffer.Length;
                Length -= size;

                if (Length == 0)
                {
                    _head = 0;
                    _fail = 0;
                }

                _sizeUntilCut = _internalBuffer.Length - _head;
                return size;
            }
        }

        /// <summary>
        /// Peeks a byte with a relative index to the fHead
        /// Note: should be used for special cases only, as it is rather slow
        /// </summary>
        /// <param name="index">A relative index</param>
        /// <returns>The byte peeked</returns>
        [PublicAPI]
        private byte PeekOne(int index)
        {
            return index >= _sizeUntilCut ? _internalBuffer[index - _sizeUntilCut] : _internalBuffer[_head + index];
        }
    }
}