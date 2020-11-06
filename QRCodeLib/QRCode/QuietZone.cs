using System;

namespace Ys.QRCode
{
    /// <summary>
    /// クワイエットゾーン
    /// </summary>
    internal static class QuietZone
    {
        public const int MIN_WIDTH = 4;

        private static int _width = MIN_WIDTH;

        public static int Width {
            get { return _width; }
            set
            {
                if (value < MIN_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _width = value;
            }
        }

        /// <summary>
        /// クワイエットゾーンを追加します。
        /// </summary>
        public static int[][] Place(int[][] moduleMatrix)
        {
            int size = moduleMatrix.Length + Width * 2;
            int[][] ret = new int[size][];

            for (int i = 0; i < size; ++i)
                ret[i] = new int[size];

            for (int i = 0; i < moduleMatrix.Length; ++i)
                moduleMatrix[i].CopyTo(ret[i + Width], Width);

            return ret;
        }
    }
}
