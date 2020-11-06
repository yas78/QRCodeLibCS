using System;

using Ys.Misc;

namespace Ys.QRCode
{
    /// <summary>
    /// シンボルマスク
    /// </summary>
    internal static class Masking
    {
        /// <summary>
        /// マスクを適用します。
        /// </summary>
        /// <param name="version">型番</param>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        /// <returns>適用されたマスクパターン参照子</returns>
        public static int Apply(
            int version, ErrorCorrectionLevel ecLevel, ref int[][] moduleMatrix)
        {
            int minPenalty = Int32.MaxValue;
            int maskPatternReference = 0;
            int[][] maskedMatrix = null;

            for (int i = 0; i <= 7; ++i)
            {
                int[][] temp = ArrayUtil.DeepCopy(moduleMatrix);

                Mask(i, temp);
                FormatInfo.Place(ecLevel, i, temp);

                if (version >= 7)
                    VersionInfo.Place(version, temp);

                int penalty = MaskingPenaltyScore.CalcTotal(temp);

                if (penalty < minPenalty)
                {
                    minPenalty = penalty;
                    maskPatternReference = i;
                    maskedMatrix = temp;
                }
            }

            moduleMatrix = maskedMatrix;
            return maskPatternReference;
        }

        /// <summary>
        /// マスクパターンを適用したシンボルデータを返します。
        /// </summary>
        /// <param name="maskPatternReference">マスクパターン参照子</param>
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        private static void Mask(int maskPatternReference, int[][] moduleMatrix)
        {
            Func<int, int, bool> condition = GetCondition(maskPatternReference);

            for (int r = 0; r < moduleMatrix.Length; ++r)
            {
                for (int c = 0; c < moduleMatrix[r].Length; ++c)
                {
                    if (Math.Abs(moduleMatrix[r][c]) == 1)
                    {
                        if(condition(r, c))
                            moduleMatrix[r][c] *= -1;
                    }
                }
            }
        }

        /// <summary>
        /// マスク条件を返します。
        /// </summary>
        /// <param name="maskPatternReference">マスクパターン参照子</param>
        private static Func<int, int, bool> GetCondition(int maskPatternReference)
        {
            switch (maskPatternReference)
            {
                case 0:
                    return (r, c) => (r + c) % 2 == 0;
                case 1:
                    return (r, c) => r % 2 == 0;
                case 2:
                    return (r, c) => c % 3 == 0;
                case 3:
                    return (r, c) => (r + c) % 3 == 0;
                case 4:
                    return (r, c) => ((r / 2) + (c / 3)) % 2 == 0;
                case 5:
                    return (r, c) => (r * c) % 2 + (r * c) % 3 == 0;
                case 6:
                    return (r, c) => ((r * c) % 2 + (r * c) % 3) % 2 == 0;
                case 7:
                    return (r, c) => ((r + c) % 2 + (r * c) % 3) % 2 == 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(maskPatternReference));
            }
        }
    }
}
