using System;

namespace Ys.TypeExtension
{
    /// <summary>
    /// Int32型の拡張メソッド
    /// </summary>
    internal static class Int32Extensions
    {
        public static int[][] CloneDeep(this int[][] arg)
        {
            int[][] ret = new int[arg.Length][];

            for (int i = 0; i < arg.Length; ++i)
            {
                ret[i] = new int[arg[i].Length];
                arg[i].CopyTo(ret[i], 0);
            }

            return ret;
        }

        public static int[][] Rotate90(this int[][] arg)
        {
            int[][] ret = new int[arg[0].Length][];

            for (int i = 0; i < ret.Length; ++i)
                ret[i] = new int[arg.Length];

            int k = ret.Length - 1;

            for (int i = 0; i < ret.Length; ++i)
                for (int j = 0; j < ret[i].Length; ++j)
                    ret[i][j] = arg[j][k - i];

            return ret;
        }
    }
}
