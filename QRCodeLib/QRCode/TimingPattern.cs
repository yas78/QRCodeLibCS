using System;

namespace Ys.QRCode
{
    /// <summary>
    /// タイミングパターン
    /// </summary>
    internal static class TimingPattern
    {
        const int VAL = Values.TIMING;

        /// <summary>
        /// タイミングパターンを配置します。
        /// </summary>
        public static void Place(int[][] moduleMatrix)
        {
            for (int i = 8; i <= moduleMatrix.Length - 9; ++i)
            {
                int v = ((i % 2 == 0) ? VAL : -VAL);

                moduleMatrix[6][i] = v;
                moduleMatrix[i][6] = v;
            }
        }
    }
}
