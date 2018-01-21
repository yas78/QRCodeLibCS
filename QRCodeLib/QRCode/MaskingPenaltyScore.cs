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
            int total   = 0;
            int penalty = 0;

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

            for (int r = 0; r < moduleMatrix.Length; ++r)
            {
                int[] columns = moduleMatrix[r];
                int cnt = 1;

                for (int c = 0; c < columns.Length - 1; ++c)
                {
                    if ((columns[c] > 0) == (columns[c + 1] > 0))
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

            for (int r = 0; r < moduleMatrix.Length - 4; ++r)
            {
                int[] columns = moduleMatrix[r];
                var startIndexes = new List<int>();

                startIndexes.Add(0);

                for (int c = 4; c < columns.Length - 2; ++c)
                {
                    if (columns[c] > 0 && columns[c + 1] <= 0)
                        startIndexes.Add(c + 1);
                }

                for (int i = 0; i < startIndexes.Count; ++i)
                {
                    int index = startIndexes[i];
                    var moduleRatio = new ModuleRatio();

                    while (index < columns.Length && columns[index] <= 0)
                    {
                        moduleRatio.PreLightRatio4++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] > 0)
                    {
                        moduleRatio.PreDarkRatio1++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] <= 0)
                    {
                        moduleRatio.PreLightRatio1++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] > 0)
                    {
                        moduleRatio.CenterDarkRatio3++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] <= 0)
                    {
                        moduleRatio.FolLightRatio1++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] > 0)
                    {
                        moduleRatio.FolDarkRatio1++;
                        index++;
                    }

                    while (index < columns.Length && columns[index] <= 0)
                    {
                        moduleRatio.FolLightRatio4++;
                        index++;
                    }

                    if (moduleRatio.PenaltyImposed())
                        penalty += 40;
                }
            }

            return penalty;
        }

        /// <summary>
        /// 全体に対する暗モジュールの占める割合について失点を計算します。
        /// </summary>
        private static int CalcProportionOfDarkModules(int[][] moduleMatrix)
        {
            int darkCount = 0;

            foreach(int[] columns in moduleMatrix)
            {
                foreach(int value in columns)
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
