using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 残余ビット
    /// </summary>
    internal static class RemainderBit
    {
        /// <summary>
        /// 残余ビットを配置します。
        /// </summary>
        public static void Place(int[][] moduleMatrix)
        {
            for (int r = 0; r < moduleMatrix.Length; ++r)
            {
                for (int c = 0; c < moduleMatrix[r].Length; ++c)
                {
                    if (moduleMatrix[r][c] == Values.BLANK)
                        moduleMatrix[r][c] = -Values.WORD;
                }
            }
        }
    }
}
