using System;
using System.Runtime.InteropServices;

namespace Ys.Image
{
    /// <summary>
    /// BITMAPINFOHEADER構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        public int     biSize;
        public int     biWidth;
        public int     biHeight;
        public short   biPlanes;
        public short   biBitCount;   
        public int     biCompression;
        public int     biSizeImage;
        public int     biXPelsPerMeter;
        public int     biYPelsPerMeter;
        public int     biClrUsed;
        public int     biClrImportant;

        /// <summary>
        /// この構造体のバイト配列を返します。
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] ret = new byte[40];

            Buffer.BlockCopy(BitConverter.GetBytes(biSize),          0, ret,  0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biWidth),         0, ret,  4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biHeight),        0, ret,  8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biPlanes),        0, ret, 12, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(biBitCount),      0, ret, 14, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(biCompression),   0, ret, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biSizeImage),     0, ret, 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biXPelsPerMeter), 0, ret, 24, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biYPelsPerMeter), 0, ret, 28, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biClrUsed),       0, ret, 32, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(biClrImportant),  0, ret, 36, 4);

            return ret;
        }
    }
}
