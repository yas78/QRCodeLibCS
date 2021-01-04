using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ys.Misc
{
    /// <summary>
    /// ビット列の生成機能を提供します。
    /// </summary>
    internal class BitSequence
    {
        List<byte> _buffer;
        int _bitCounter;
        int _space;

        public BitSequence()
        {
            Clear();
        }

        /// <summary>
        /// ビット数を取得します。
        /// </summary>
        public int Length => _bitCounter;

        public void Clear()
        {
            _buffer = new List<byte>();
            _bitCounter = 0;
            _space      = 0;
        }

        /// <summary>
        /// 指定のビット数でデータを追加します。
        /// </summary>
        /// <param name="data">追加するデータ</param>
        /// <param name="length">データのビット数</param>
        public void Append(int data, int length)
        {            
            int remainingLength = length;
            int remainingData = data;

            while (remainingLength > 0)
            {
                if (_space == 0)
                {
                    _space = 8;
                    _buffer.Add(0x0);
                }

                if (_space < remainingLength)
                {
                    _buffer[_buffer.Count - 1] |= (byte)(remainingData >> (remainingLength - _space));

                    remainingData &= (1 << (remainingLength - _space)) - 1;

                    _bitCounter += _space;
                    remainingLength -= _space;
                    _space = 0;
                }
                else
                {
                    _buffer[_buffer.Count - 1] |= (byte)(remainingData << (_space - remainingLength));

                    _bitCounter += remainingLength;
                    _space -= remainingLength;
                    remainingLength = 0;
                }
            }
        }

        /// <summary>
        /// データのバイト配列を返します。
        /// </summary>
        public byte[] GetBytes()
        {
            return _buffer.ToArray();
        }
    }
}
