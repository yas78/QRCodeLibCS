using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 位置検出パターン
    /// </summary>
    internal static class FinderPattern
    {
        const int VAL = Values.FINDER;

        // 位置検出パターン
        static readonly int[][] _finderPattern = {
            new [] {VAL,  VAL,  VAL,  VAL,  VAL,  VAL,  VAL},
            new [] {VAL, -VAL, -VAL, -VAL, -VAL, -VAL,  VAL},
            new [] {VAL, -VAL,  VAL,  VAL,  VAL, -VAL,  VAL},
            new [] {VAL, -VAL,  VAL,  VAL,  VAL, -VAL,  VAL},
            new [] {VAL, -VAL,  VAL,  VAL,  VAL, -VAL,  VAL},
            new [] {VAL, -VAL, -VAL, -VAL, -VAL, -VAL,  VAL},
            new [] {VAL,  VAL , VAL,  VAL,  VAL,  VAL,  VAL}
        };

        /// <summary>
        /// 位置検出パターンを配置します。
        /// </summary>
        public static void Place(int[][] moduleMatrix)
        {
            int offset = moduleMatrix.Length - _finderPattern.Length;

            for (int i = 0; i < _finderPattern.Length; ++i)
            {
                for (int j = 0; j < _finderPattern[i].Length; ++j)
                {
                    int v = _finderPattern[i][j];

                    moduleMatrix[i][j] = v;
                    moduleMatrix[i][j + offset] = v;
                    moduleMatrix[i + offset][j] = v;
                }
            }
        }
    }
}
