﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ys.Util
{
    /// <summary>
    /// ビット列の生成機能を提供します。
    /// </summary>
    internal class BitSequence
    {
        readonly List<byte> _buffer = new List<byte>();

        int _bitCounter = 0;
        int _space      = 0;

        public BitSequence() { }

        /// <summary>
        /// ビット数を取得します。
        /// </summary>
        public int Length
        {
            get { return _bitCounter; }
        }

        /// <summary>
        /// 指定のビット数でデータを追加します。
        /// </summary>
        /// <param name="data">追加するデータ</param>
        /// <param name="length">データのビット数</param>
        public void Append(int data, int length)
        {
            Debug.Assert(data >= 0);
            Debug.Assert(length >= 0);
            
            int remainingLength = length;
            int remainingData   = data;

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
                    
                    _bitCounter     += _space;
                    remainingLength -= _space;
                    _space          = 0;
                }
                else
                {
                    _buffer[_buffer.Count - 1] |= (byte)(remainingData << (_space - remainingLength));

                    _bitCounter     += remainingLength;
                    _space          -= remainingLength;
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