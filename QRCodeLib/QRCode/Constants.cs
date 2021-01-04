using System;

namespace Ys.QRCode
{
    public static class Constants
    {
        public const int MIN_VERSION = 1;
        public const int MAX_VERSION = 40;
    }

    internal static class Values
    {
        public const int BLANK      = 0;
        public const int WORD       = 1;
        public const int ALIGNMENT  = 2;
        public const int FINDER     = 3; 
        public const int FORMAT     = 4;
        public const int SEPARATOR  = 5; 
        public const int TIMING     = 6;
        public const int VERSION    = 7;

        public static bool IsDark(int value)
        {
            return value > BLANK;
        }
    }
}
