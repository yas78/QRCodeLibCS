using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 分離パターン
    /// </summary>
    internal static class Separator
    {
        /// <summary>
        /// 分離パターンを配置します。
        /// </summary>
        public static void Place(int[][] moduleMatrix)
        {
            int offset = moduleMatrix.Length - 8;

            for (int i = 0; i <= 7; ++i)
            {
                moduleMatrix[i][7]          = -2;
                moduleMatrix[7][i]          = -2;

                moduleMatrix[offset + i][7] = -2;
                moduleMatrix[offset + 0][i] = -2;

                moduleMatrix[i][offset + 0] = -2;
                moduleMatrix[7][offset + i] = -2;
            }
        }
    }
}
