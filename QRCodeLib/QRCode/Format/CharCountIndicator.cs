using System;

namespace Ys.QRCode.Format
{
    /// <summary>
    /// 文字数指示子
    /// </summary>
    internal static class CharCountIndicator
    {
        /// <summary>
        /// 文字数指示子のビット数を返します。
        /// </summary>
        /// <param name="version">型番</param>
        /// <param name="encMode">符号化モード</param>
        public static int GetLength(int version, EncodingMode encMode)
        {
            if (1 <= version && version <= 9)
            {
                switch (encMode)
                {
                    case EncodingMode.NUMERIC:
                        return 10;
                    case EncodingMode.ALPHA_NUMERIC:
                        return 9;
                    case EncodingMode.EIGHT_BIT_BYTE:
                        return 8;
                    case EncodingMode.KANJI:
                        return 8;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(encMode));
                }
            }
            else if (10 <= version && version <= 26)
            {
                switch (encMode)
                {
                    case EncodingMode.NUMERIC:
                        return 12;
                    case EncodingMode.ALPHA_NUMERIC:
                        return 11;
                    case EncodingMode.EIGHT_BIT_BYTE:
                        return 16;
                    case EncodingMode.KANJI:
                        return 10;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(encMode));
                }
            }
            else if (27 <= version && version <= 40)
            {
                switch (encMode)
                {
                    case EncodingMode.NUMERIC:
                        return 14;
                    case EncodingMode.ALPHA_NUMERIC:
                        return 13;
                    case EncodingMode.EIGHT_BIT_BYTE:
                        return 16;
                    case EncodingMode.KANJI:
                        return 12;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(encMode));
                }
            }
            else
                throw new ArgumentOutOfRangeException(nameof(version));
        }
    }
}
