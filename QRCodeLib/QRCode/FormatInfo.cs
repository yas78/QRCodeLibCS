using System;
using System.Diagnostics;

namespace Ys.QRCode
{
    /// <summary>
    /// 形式情報
    /// </summary>
    internal static class FormatInfo
    {
        // 形式情報
        static int[] _formatInfoValues = {
            0x0000, 0x0537, 0x0A6E, 0x0F59, 0x11EB, 0x14DC, 0x1B85, 0x1EB2, 0x23D6, 0x26E1,
            0x29B8, 0x2C8F, 0x323D, 0x370A, 0x3853, 0x3D64, 0x429B, 0x47AC, 0x48F5, 0x4DC2,
            0x5370, 0x5647, 0x591E, 0x5C29, 0x614D, 0x647A, 0x6B23, 0x6E14, 0x70A6, 0x7591,
            0x7AC8, 0x7FFF
        };

        // 形式情報のマスクパターン
        static int[] _formatInfoMaskArray =  {
            0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1
        };

        /// <summary>
        /// 形式情報を配置します。
        /// </summary>
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="maskPatternReference">マスクパターン参照子</param>
        public static void Place(
            int[][] moduleMatrix, ErrorCorrectionLevel ecLevel, int maskPatternReference)
        {
            int formatInfoValue = GetFormatInfoValue(ecLevel, maskPatternReference);

            int r1 = 0;
            int c1 = moduleMatrix.Length - 1;

            for (int i = 0; i <= 7; ++i)
            {
                int temp = ((formatInfoValue & (1 << i)) > 0 ? 1 : 0) ^ _formatInfoMaskArray[i];

                int v = (temp > 0) ? 3 : -3;

                moduleMatrix[r1][8 ] = v;
                moduleMatrix[8 ][c1] = v;

                r1++;
                c1--;

                if (r1 == 6)
                    r1++;
            }

            int r2 = moduleMatrix.Length - 7;
            int c2 = 7;

            for (int i = 8; i <= 14; ++i)
            {
                int temp = ((formatInfoValue & (1 << i)) > 0 ? 1 : 0) ^ _formatInfoMaskArray[i];

                int v = (temp > 0) ? 3 : -3;

                moduleMatrix[r2][8 ] = v;
                moduleMatrix[8 ][c2] = v;

                r2++;
                c2--;

                if (c2 == 6)
                    c2--;
            }
        }
        
        /// <summary>
        /// 形式情報の予約領域を配置します。
        /// </summary>
        public static void PlaceTempBlank(int[][] moduleMatrix)
        {
            int numModulesOneSide = moduleMatrix.Length;

            for (int i = 0; i <= 8; ++i)
            {
                // タイミグパターンの領域ではない場合
                if (i != 6)
                {
                    moduleMatrix[8][i] = -3;
                    moduleMatrix[i][8] = -3;
                }
            }

            for (int i = numModulesOneSide - 8; i <= numModulesOneSide - 1; ++i)
            {
                moduleMatrix[8][i] = -3;
                moduleMatrix[i][8] = -3;
            }

            // 固定暗モジュールを配置(マスクの適用前に配置する)
            moduleMatrix[numModulesOneSide - 8][8] = 2;
        }

        /// <summary>
        /// 形式情報の値を取得します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="maskPatternReference">マスクパターン参照子</param>
        public static int GetFormatInfoValue(
            ErrorCorrectionLevel ecLevel, int maskPatternReference)
        {
            int indicator;

            switch (ecLevel)
            {
                case ErrorCorrectionLevel.L:
                    indicator = 1;
                    break;
                case ErrorCorrectionLevel.M:
                    indicator = 0;
                    break;
                case ErrorCorrectionLevel.Q:
                    indicator = 3;
                    break;
                case ErrorCorrectionLevel.H:
                    indicator = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ecLevel));
            }

            return _formatInfoValues[(indicator << 3) | maskPatternReference];
        }
    }
}
