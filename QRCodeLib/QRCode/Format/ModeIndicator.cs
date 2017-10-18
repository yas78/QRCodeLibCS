using System;

namespace Ys.QRCode.Format
{
    /// <summary>
    /// モード指示子
    /// </summary>
    internal static class ModeIndicator
    {
        public const int LENGTH = 4;

        public const int TERMINATOR_VALUE         = 0x0;
        public const int NUMERIC_VALUE            = 0x1;
        public const int ALPAHNUMERIC_VALUE       = 0x2;
        public const int STRUCTURED_APPEND_VALUE  = 0x3;
        public const int BYTE_VALUE               = 0x4;
        public const int KANJI_VALUE              = 0x8;
    }
}
