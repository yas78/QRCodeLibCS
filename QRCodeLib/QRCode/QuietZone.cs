using System;

namespace Ys.QRCode
{
    /// <summary>
    /// クワイエットゾーン
    /// </summary>
    internal static class QuietZone
    {
        public const int WIDTH = 4;

        /// <summary>
        /// クワイエットゾーンを追加します。
        /// </summary>
        public static int[][] Place(int[][] moduleMatrix)
        {
            int size = moduleMatrix.Length + WIDTH * 2;
            int[][] ret = new int[size][];

            for (int i = 0; i < size; ++i)
                ret[i] = new int[size];

            for (int i = 0; i < moduleMatrix.Length; ++i)
                moduleMatrix[i].CopyTo(ret[i + WIDTH], WIDTH);

            return ret;
        }
    }
}
