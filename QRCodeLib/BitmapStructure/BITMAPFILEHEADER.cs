using System;
using System.Runtime.InteropServices;

namespace Ys.BitmapStructure
{
    /// <summary>
    /// BITMAPFILEHEADER構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct BITMAPFILEHEADER
    {
        public short   bfType;
        public int     bfSize;
        public short   bfReserved1;
        public short   bfReserved2;
        public int     bfOffBits; 
        
        /// <summary>
        /// この構造体のバイト配列を返します。
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] ret = new byte[14];

            Buffer.BlockCopy(BitConverter.GetBytes(bfType),      0, ret,  0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bfSize),      0, ret,  2, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bfReserved1), 0, ret,  6, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bfReserved2), 0, ret,  8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bfOffBits),   0, ret, 10, 4);

            return ret;
        }
    }
}
