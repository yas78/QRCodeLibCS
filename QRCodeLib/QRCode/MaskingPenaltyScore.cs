using System;
using System.Collections.Generic;

using Ys.TypeExtension;

namespace Ys.QRCode
{
    /// <summary>
    /// マスクされたシンボルの失点評価
    /// </summary>
    internal static class MaskingPenaltyScore
    {
        /// <summary>
        /// マスクパターン失点の合計を返します。
        /// </summary>
        public static int CalcTotal(int[][] moduleMatrix)
        {
            int total = 0;
            int penalty;

            penalty = CalcAdjacentModulesInSameColor(moduleMatrix);
            total += penalty;

            penalty = CalcBlockOfModulesInSameColor(moduleMatrix);
            total += penalty;

            penalty = CalcModuleRatio(moduleMatrix);
            total += penalty;

            penalty = CalcProportionOfDarkModules(moduleMatrix);
            total += penalty;

            return total;
        }

        /// <summary>
        /// 行／列の同色隣接モジュールパターンの失点を計算します。
        /// </summary>
        private static int CalcAdjacentModulesInSameColor(int[][] moduleMatrix)
        {
            int penalty = 0;

            penalty += CalcAdjacentModulesInRowInSameColor(moduleMatrix);
            penalty += CalcAdjacentModulesInRowInSameColor(moduleMatrix.Rotate90());

            return penalty;
        }

        /// <summary>
        /// 行の同色隣接モジュールパターンの失点を計算します。
        /// </summary>
        private static int CalcAdjacentModulesInRowInSameColor(int[][] moduleMatrix)
        {
            int penalty = 0;

            foreach (int[] row in moduleMatrix)
            {
                int cnt = 1;

                for (int c = 0; c < row.Length - 1; ++c)
                {
                    if ((row[c] > 0) == (row[c + 1] > 0))
                        cnt++;
                    else
                    {
                        if (cnt >= 5)
                            penalty += 3 + (cnt - 5);

                        cnt = 1;
                    }
                }

                if (cnt >= 5)
                    penalty += 3 + (cnt - 5);
            }

            return penalty;
        }

        /// <summary>
        /// 2x2の同色モジュールパターンの失点を計算します。
        /// </summary>
        private static int CalcBlockOfModulesInSameColor(int[][] moduleMatrix)
        {
            int penalty = 0;

            for (int r = 0; r < moduleMatrix.Length - 1; ++r)
            {
                for (int c = 0; c < moduleMatrix[r].Length - 1; ++c)
                {
                    bool temp = moduleMatrix[r][c] > 0;
                    bool isSameColor = true;

                    isSameColor &= moduleMatrix[r + 0][c + 1] > 0 == temp;
                    isSameColor &= moduleMatrix[r + 1][c + 0] > 0 == temp;
                    isSameColor &= moduleMatrix[r + 1][c + 1] > 0 == temp;

                    if (isSameColor)
                        penalty += 3;
                }
            }

            return penalty;
        }

        /// <summary>
        /// 行／列における1 : 1 : 3 : 1 : 1 比率パターンの失点を計算します。
        /// </summary>
        private static int CalcModuleRatio(int[][] moduleMatrix)
        {
            int[][] moduleMatrixTemp = QuietZone.Place(moduleMatrix);

            int penalty = 0;

            penalty += CalcModuleRatioInRow(moduleMatrixTemp);
            penalty += CalcModuleRatioInRow(moduleMatrixTemp.Rotate90());

            return penalty;
        }
        
        /// <summary>
        /// 行の1 : 1 : 3 : 1 : 1 比率パターンの失点を計算します。
        /// </summary>
        private static int CalcModuleRatioInRow(int[][] moduleMatrix)
        {
            int penalty = 0;

            foreach (int[] row in moduleMatrix)
            {
                int[][] ratio3Ranges = GetRatio3Ranges(row);

                foreach (int[] rng in ratio3Ranges)
                {
                    int ratio3 = rng[1] + 1 - rng[0];
                    int ratio1 = ratio3 / 3;
                    int ratio4 = ratio1 * 4;
                    bool impose = false;
                    int cnt;

                    int i = rng[0] - 1;

                    // light ratio 1
                    for (cnt = 0; i >= 0 && row[i] <= 0; ++cnt, --i);

                    if (cnt != ratio1)
                        continue;

                    // dark ratio 1
                    for (cnt = 0; i >= 0 && row[i] > 0; ++cnt, --i);

                    if (cnt != ratio1)
                        continue;

                    // light ratio 4
                    for (cnt = 0; i >= 0 && row[i] <= 0; ++cnt, --i);

                    if (cnt >= ratio4)
                        impose = true;

                    i = rng[1] + 1;

                    // light ratio 1
                    for (cnt = 0; i <= row.Length - 1 && row[i] <= 0; ++cnt, ++i);

                    if (cnt != ratio1)
                        continue;

                    // dark ratio 1
                    for (cnt = 0; i <= row.Length - 1 && row[i] > 0; ++cnt, ++i);

                    if (cnt != ratio1)
                        continue;

                    // light ratio 4
                    for (cnt = 0; i <= row.Length - 1 && row[i] <= 0; ++cnt, ++i);

                    if (cnt >= ratio4)
                        impose = true;

                    if (impose)
                        penalty += 40;
                }
            }

            return penalty;
        }

        private static int[][] GetRatio3Ranges(int[] arg)
        {
            var ret = new List<int[]>();
            int s = 0;
            int e;

            for (int i = QuietZone.WIDTH; i < arg.Length - QuietZone.WIDTH; ++i)
            {
                if (arg[i] > 0 && arg[i - 1] <= 0)
                    s = i;

                if (arg[i] > 0 && arg[i + 1] <= 0)
                {
                    e = i;

                    if ((e + 1 - s) % 3 == 0)
                        ret.Add(new[] { s, e });
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// 全体に対する暗モジュールの占める割合について失点を計算します。
        /// </summary>
        private static int CalcProportionOfDarkModules(int[][] moduleMatrix)
        {
            int darkCount = 0;

            foreach(int[] row in moduleMatrix)
            {
                foreach(int value in row)
                {
                    if (value > 0)
                        darkCount++;
                }
            }

            double numModules = Math.Pow(moduleMatrix.Length, 2);
            int temp;
            temp = (int)Math.Ceiling(darkCount / numModules * 100);
            temp = Math.Abs(temp - 50);
            temp = (temp + 4) / 5;

            return temp * 10;
        }
    }
}
