using System;
using System.Runtime.InteropServices;

namespace Ys.Image
{
    /// <summary>
    /// RGBQUAD構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;

        /// <summary>
        /// この構造体のバイト配列を返します。
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] ret = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(rgbBlue),     0, ret,  0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(rgbGreen),    0, ret,  1, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(rgbRed),      0, ret,  2, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(rgbReserved), 0, ret,  3, 1);

            return ret;
        }
    }
}
