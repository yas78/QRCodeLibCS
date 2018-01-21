﻿using System;
using System.Diagnostics;

namespace Ys.QRCode.Format
{
    /// <summary>
    /// RSブロック
    /// </summary>
    internal static class RSBlock
    {
        /// <summary>
        /// RSブロック数を返します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="version">型番</param>
        /// <param name="preceding">RSブロック前半部分は true を指定します。</param>
        public static int GetTotalNumber(
            ErrorCorrectionLevel ecLevel, int version, bool preceding)
        {
            Debug.Assert(version >= Constants.MIN_VERSION && 
                         version <= Constants.MAX_VERSION);

            int numDataCodewords = DataCodeword.GetTotalNumber(ecLevel, version);
            int numRSBlocks = _totalNumbers[(int)ecLevel][version];

            int numFolBlocks = numDataCodewords % numRSBlocks;

            if (preceding)
                return numRSBlocks - numFolBlocks;
            else
                return numFolBlocks;
        }

        /// <summary>
        /// RSブロックのデータコード語数を返します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="version">型番</param>
        /// <param name="preceding">RSブロック前半部分は true を指定します。</param>
        public static int GetNumberDataCodewords(
            ErrorCorrectionLevel ecLevel, int version, bool preceding)
        {
            Debug.Assert(version >= Constants.MIN_VERSION && 
                         version <= Constants.MAX_VERSION);

            int numDataCodewords = DataCodeword.GetTotalNumber(ecLevel, version);
            int numRSBlocks = _totalNumbers[(int)ecLevel][version];

            int numPreBlockCodewords = numDataCodewords / numRSBlocks;

            if (preceding)
                return numPreBlockCodewords;
            else
            {
                int numPreBlocks = GetTotalNumber(ecLevel, version, true);
                int numFolBlocks = GetTotalNumber(ecLevel, version, false);

                if (numFolBlocks > 0)
                    return (numDataCodewords - numPreBlockCodewords * numPreBlocks) / numFolBlocks;
                else
                    return 0;
            }
        }

        /// <summary>
        /// RSブロックの誤り訂正コード語数を返します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="version">型番</param>
        public static int GetNumberECCodewords(
            ErrorCorrectionLevel ecLevel, int version)
        {
            Debug.Assert(version >= Constants.MIN_VERSION && 
                         version <= Constants.MAX_VERSION);

            int numDataCodewords = DataCodeword.GetTotalNumber(ecLevel, version);
            int numRSBlocks = _totalNumbers[(int)ecLevel][version];

            return (Codeword.GetTotalNumber(version) / numRSBlocks) -
                        (numDataCodewords / numRSBlocks);
        }

        // RSブロック数
        private static readonly int[][] _totalNumbers = {
            /* Error Correction Level L */
            new []{
                -1,
                 1,  1,  1,  1,  1,  2,  2,  2,  2,  4,
                 4,  4,  4,  4,  6,  6,  6,  6,  7,  8,
                 8,  9,  9, 10, 12, 12, 12, 13, 14, 15,
                16, 17, 18, 19, 19, 20, 21, 22, 24, 25
            },
            /* Error Correction Level M */
            new []{
                -1,
                 1,  1,  1,  2,  2,  4,  4,  4,  5,  5,
                 5,  8,  9,  9, 10, 10, 11, 13, 14, 16,
                17, 17, 18, 20, 21, 23, 25, 26, 28, 29,
                31, 33, 35, 37, 38, 40, 43, 45, 47, 49
            },
            /* Error Correction Level Q */
            new []{
                -1,
                 1,  1,  2,  2,  4,  4,  6,  6,  8,  8,
                 8, 10, 12, 16, 12, 17, 16, 18, 21, 20,
                23, 23, 25, 27, 29, 34, 34, 35, 38, 40,
                43, 45, 48, 51, 53, 56, 59, 62, 65, 68
            },
            /* Error Correction Level H */
            new []{
                -1,
                 1,  1,  2,  4,  4,  4,  5,  6,  8,  8,
                11, 11, 16, 16, 18, 16, 19, 21, 25, 25,
                25, 34, 30, 32, 35, 37, 40, 42, 45, 48,
                51, 54, 57, 60, 63, 66, 70, 74, 77, 81
            }
        };
    }
}
