using System;
using System.Diagnostics;

namespace Ys.QRCode
{
    /// <summary>
    /// 位置合わせパターン
    /// </summary>
    internal static class AlignmentPattern
    {
        /// <summary>
        ///    位置合わせパターンを配置します。
        ///    </summary>
        public static void Place(int[][] moduleMatrix, int version)
        {
            Debug.Assert(version >= 2 && version <= 40);

            int[] centerPosArray = _centerPosArrays[version];

            int maxIndex = centerPosArray.Length - 1;

            for (int i = 0; i <= maxIndex; ++i)
            {
                int r = centerPosArray[i];

                for (int j = 0; j <= maxIndex; ++j)
                {
                    int c = centerPosArray[j];

                    // 位置検出パターンと重なる場合
                    if (i == 0        && j == 0        || 
                        i == 0        && j == maxIndex || 
                        i == maxIndex && j == 0)
                    {
                        continue;
                    }
                        
                    Array.Copy(new[] { 2,  2,  2,  2,  2 }, 0, moduleMatrix[r - 2], c - 2, 5);
                    Array.Copy(new[] { 2, -2, -2, -2,  2 }, 0, moduleMatrix[r - 1], c - 2, 5);
                    Array.Copy(new[] { 2, -2,  2, -2,  2 }, 0, moduleMatrix[r + 0], c - 2, 5);
                    Array.Copy(new[] { 2, -2, -2, -2,  2 }, 0, moduleMatrix[r + 1], c - 2, 5);
                    Array.Copy(new[] { 2,  2,  2,  2,  2 }, 0, moduleMatrix[r + 2], c - 2, 5);
                }
            }
        }

        // 位置合せパターンの中心座標
        private static readonly int[][] _centerPosArrays = {
            null,
            null,
            new[]{6, 18},
            new[]{6, 22},
            new[]{6, 26},
            new[]{6, 30},
            new[]{6, 34},
            new[]{6, 22, 38},
            new[]{6, 24, 42},
            new[]{6, 26, 46},
            new[]{6, 28, 50},
            new[]{6, 30, 54},
            new[]{6, 32, 58},
            new[]{6, 34, 62},
            new[]{6, 26, 46, 66},
            new[]{6, 26, 48, 70},
            new[]{6 ,26, 50, 74},
            new[]{6 ,30, 54, 78},
            new[]{6, 30, 56, 82},
            new[]{6, 30, 58, 86},
            new[]{6, 34, 62, 90},
            new[]{6, 28, 50, 72, 94},
            new[]{6, 26, 50, 74, 98},
            new[]{6, 30, 54, 78, 102},
            new[]{6, 28, 54, 80, 106},
            new[]{6, 32, 58, 84, 110},
            new[]{6, 30, 58, 86, 114},
            new[]{6, 34, 62, 90, 118},
            new[]{6, 26, 50, 74, 98, 122},
            new[]{6, 30, 54, 78, 102, 126},
            new[]{6, 26, 52, 78, 104, 130},
            new[]{6, 30, 56, 82, 108, 134},
            new[]{6, 34, 60, 86, 112, 138},
            new[]{6, 30, 58, 86, 114, 142},
            new[]{6, 34, 62, 90, 118, 146},
            new[]{6, 30, 54, 78, 102, 126, 150},
            new[]{6, 24, 50, 76, 102, 128, 154},
            new[]{6, 28, 54, 80, 106, 132, 158},
            new[]{6, 32, 58, 84, 110, 136, 162},
            new[]{6, 26, 54, 82, 110, 138, 166},
            new[]{6, 30, 58, 86, 114, 142, 170}
        };
    }
}
