using System;
using System.Drawing;

namespace Ys.Image
{
    internal static class DIB
    {
        public static byte[] Build1bppDIB(
            byte[] bitmapData, int width, int height, Color foreColor, Color backColor)
        {
            BITMAPFILEHEADER bfh;
            bfh.bfType         = 0x4D42;
            bfh.bfSize         = 62 + bitmapData.Length;
            bfh.bfReserved1    = 0;
            bfh.bfReserved2    = 0;
            bfh.bfOffBits      = 62;

            BITMAPINFOHEADER bih;
            bih.biSize             = 40;
            bih.biWidth            = width;
            bih.biHeight           = height;
            bih.biPlanes           = 1;
            bih.biBitCount         = 1;
            bih.biCompression      = 0;
            bih.biSizeImage        = 0;
            bih.biXPelsPerMeter    = 3780; // 96dpi
            bih.biYPelsPerMeter    = 3780; // 96dpi
            bih.biClrUsed          = 0;
            bih.biClrImportant     = 0;
            
            RGBQUAD[] palette = new RGBQUAD[2];
            palette[0].rgbBlue     = foreColor.B;
            palette[0].rgbGreen    = foreColor.G;
            palette[0].rgbRed      = foreColor.R;
            palette[0].rgbReserved = 0;
            palette[1].rgbBlue     = backColor.B;
            palette[1].rgbGreen    = backColor.G;
            palette[1].rgbRed      = backColor.R;
            palette[1].rgbReserved = 0; 

            byte[] ret = new byte[62 + bitmapData.Length];

            byte[] bytes;
            int offset= 0;
            
            bytes = bfh.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = bih.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = palette[0].GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = palette[1].GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = bitmapData;
            Buffer.BlockCopy(bytes, 0, ret,  offset, bytes.Length);

            return ret;
        }

        public static byte[] Build24bppDIB(byte[] bitmapData, int width, int height)
        {
            BITMAPFILEHEADER bfh;
            bfh.bfType         = 0x4D42;
            bfh.bfSize         = 54 + bitmapData.Length;
            bfh.bfReserved1    = 0;
            bfh.bfReserved2    = 0;
            bfh.bfOffBits      = 54;

            BITMAPINFOHEADER bih;
            bih.biSize             = 40;
            bih.biWidth            = width;
            bih.biHeight           = height;
            bih.biPlanes           = 1;
            bih.biBitCount         = 24;
            bih.biCompression      = 0;
            bih.biSizeImage        = 0;
            bih.biXPelsPerMeter    = 3780; // 96dpi
            bih.biYPelsPerMeter    = 3780; // 96dpi
            bih.biClrUsed          = 0;
            bih.biClrImportant     = 0;
                
            byte[] ret = new byte[54 + bitmapData.Length];
            byte[] bytes;
            int    offset= 0;
            
            bytes = bfh.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = bih.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;
                
            bytes = bitmapData;
            Buffer.BlockCopy(bytes, 0, ret,  offset, bytes.Length);

            return ret;
        }
    }
}
