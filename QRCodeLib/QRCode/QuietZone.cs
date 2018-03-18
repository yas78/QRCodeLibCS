using System;

namespace Ys.QRCode
{
    /// <summary>
    /// クワイエットゾーン
    /// </summary>
    internal static class QuietZone
    {
        const int QUIET_ZONE_WIDTH = 4;

        /// <summary>
        /// クワイエットゾーンを追加します。
        /// </summary>
        public static int[][] Place(int[][] moduleMatrix)
        {
            int[][] ret = new int[moduleMatrix.Length + QUIET_ZONE_WIDTH * 2][];

            for (int i = 0; i < ret.Length; ++i)
                ret[i] = new int[ret.Length];

            for (int i = 0; i < moduleMatrix.Length; ++i)
                moduleMatrix[i].CopyTo(ret[i + QUIET_ZONE_WIDTH], QUIET_ZONE_WIDTH);

            return ret;
        }
    }
}
