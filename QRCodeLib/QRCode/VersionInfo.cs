using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 型番情報
    /// </summary>
    internal static class VersionInfo
    {
        // 型番情報
        static readonly int[] _versionInfoValues = {
            -1, -1, -1, -1, -1, -1, -1,
            0x07C94, 0x085BC, 0x09A99, 0x0A4D3, 0x0BBF6, 0x0C762, 0x0D847, 0x0E60D,
            0x0F928, 0x10B78, 0x1145D, 0x12A17, 0x13532, 0x149A6, 0x15683, 0x168C9,
            0x177EC, 0x18EC4, 0x191E1, 0x1AFAB, 0x1B08E, 0x1CC1A, 0x1D33F, 0x1ED75,
            0x1F250, 0x209D5, 0x216F0, 0x228BA, 0x2379F, 0x24B0B, 0x2542E, 0x26A64,
            0x27541, 0x28C69
        };

        /// <summary>
        /// 型番情報を配置します。
        /// </summary>
        public static void Place(int version, int[][] moduleMatrix)
        {
            int numModulesPerSide = moduleMatrix.Length;

            int versionInfoValue = _versionInfoValues[version];

            int p1 = 0;
            int p2 = numModulesPerSide - 11;

            for (int i = 0; i < 18; ++i)
            {
                int v = (versionInfoValue & (1 << i)) > 0 ? 3 : -3;

                moduleMatrix[p1][p2] = v;
                moduleMatrix[p2][p1] = v;

                p2++;

                if (i % 3 == 2)
                {
                    p1++;
                    p2 = numModulesPerSide - 11;
                }
            }
        }

        /// <summary>
        /// 型番情報の予約領域を配置します。
        /// </summary>
        public static void PlaceTempBlank(int[][] moduleMatrix)
        {
            int numModulesPerSide = moduleMatrix.Length;

            for (int i = 0; i <= 5; ++i)
            {
                for (int j = numModulesPerSide - 11; j <= numModulesPerSide - 9; ++j)
                {
                    moduleMatrix[i][j] = -3;
                    moduleMatrix[j][i] = -3;
                }
            }
        }
    }
}
