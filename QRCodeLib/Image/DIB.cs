using System;
using System.Drawing;

namespace Ys.Image
{
    internal static class DIB
    {
        public static byte[] Build1bppDIB(
            byte[] bitmapData, int width, int height, Color foreColor, Color backColor)
        {
            var bfh = new BITMAPFILEHEADER() { 
                bfType         = 0x4D42,
                bfSize         = 62 + bitmapData.Length,
                bfReserved1    = 0,
                bfReserved2    = 0,
                bfOffBits      = 62
            };

            var bih = new BITMAPINFOHEADER() { 
                biSize             = 40,
                biWidth            = width,
                biHeight           = height,
                biPlanes           = 1,
                biBitCount         = 1,
                biCompression      = 0,
                biSizeImage        = 0,
                biXPelsPerMeter    = 0,
                biYPelsPerMeter    = 0,
                biClrUsed          = 0,
                biClrImportant     = 0
            };
            
            var palette = new RGBQUAD[]{
                new RGBQUAD(){
                    rgbBlue     = foreColor.B,
                    rgbGreen    = foreColor.G,
                    rgbRed      = foreColor.R,
                    rgbReserved = 0
                },
                new RGBQUAD(){
                    rgbBlue     = backColor.B,
                    rgbGreen    = backColor.G,
                    rgbRed      = backColor.R,
                    rgbReserved = 0
                }
            };

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
            var bfh = new BITMAPFILEHEADER() { 
                bfType         = 0x4D42,
                bfSize         = 54 + bitmapData.Length,
                bfReserved1    = 0,
                bfReserved2    = 0,
                bfOffBits      = 54
            };

            var bih = new BITMAPINFOHEADER(){
                biSize             = 40,
                biWidth            = width,
                biHeight           = height,
                biPlanes           = 1,
                biBitCount         = 24,
                biCompression      = 0,
                biSizeImage        = 0,
                biXPelsPerMeter    = 0,
                biYPelsPerMeter    = 0,
                biClrUsed          = 0,
                biClrImportant     = 0
            };
                
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
