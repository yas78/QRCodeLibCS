using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 分離パターン
    /// </summary>
    internal static class Separator
    {
        const int VAL = Values.SEPARATOR;

        /// <summary>
        /// 分離パターンを配置します。
        /// </summary>
        public static void Place(int[][] moduleMatrix)
        {
            int offset = moduleMatrix.Length - 8;

            for (int i = 0; i <= 7; ++i)
            {
                moduleMatrix[i][7] = -VAL;
                moduleMatrix[7][i] = -VAL;

                moduleMatrix[offset + i][7] = -VAL;
                moduleMatrix[offset + 0][i] = -VAL;

                moduleMatrix[i][offset + 0] = -VAL;
                moduleMatrix[7][offset + i] = -VAL;
            }
        }
    }
}
