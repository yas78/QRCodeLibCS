using System;
using System.Diagnostics;

using Ys.TypeExtension;

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
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        /// <param name="version">型番</param>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <returns>適用されたマスクパターン参照子</returns>
        public static int Apply(
            int[][] moduleMatrix, int version, ErrorCorrectionLevel ecLevel)
        {
            int maskPatternReference = SelectMaskPattern(moduleMatrix, version, ecLevel);
            Mask(moduleMatrix, maskPatternReference);

            return maskPatternReference;
        }

        /// <summary>
        /// マスクパターンを決定します。
        /// </summary>
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        /// <param name="version">型番</param>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <returns>マスクパターン参照子</returns>
        private static int SelectMaskPattern(
            int[][] moduleMatrix, int version, ErrorCorrectionLevel ecLevel)
        {
            int minPenalty = Int32.MaxValue;
            int ret = 0;

            for (int maskPatternReference = 0; maskPatternReference <= 7; ++maskPatternReference)
            {
                int[][] moduleMatrixClone = moduleMatrix.CloneDeep();

                Mask(moduleMatrixClone, maskPatternReference);

                FormatInfo.Place(moduleMatrixClone, ecLevel, maskPatternReference);

                if (version >= 7)
                    VersionInfo.Place(moduleMatrixClone, version);

                int penalty = MaskingPenaltyScore.CalcTotal(moduleMatrixClone);

                if (penalty < minPenalty)
                {
                    minPenalty = penalty;
                    ret = maskPatternReference;
                }
            }

            return ret;
        }

        /// <summary>
        /// マスクパターンを適用したシンボルデータを返します。
        /// </summary>
        /// <param name="moduleMatrix">シンボルの明暗パターン</param>
        /// <param name="maskPatternReference">マスクパターン参照子</param>
        private static void Mask(int[][] moduleMatrix, int maskPatternReference)
        {
            Debug.Assert(maskPatternReference >= 0 && maskPatternReference <= 7);

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
