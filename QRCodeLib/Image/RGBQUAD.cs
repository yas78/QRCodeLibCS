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
            return new byte[] { rgbBlue,rgbGreen,rgbRed,rgbReserved };
        }
    }
}
