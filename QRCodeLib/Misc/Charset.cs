using System;
using System.Linq;

namespace Ys.Misc
{
    internal class Charset
    {
        public const string SHIFT_JIS   = "Shift_JIS";
        public const string GB2312      = "GB2312";
        public const string EUC_KR      = "EUC-KR";

        static readonly string[] cjkCharsetNames = { SHIFT_JIS, GB2312, EUC_KR };

        public static bool IsJP(string charsetName)
        {
            if (string.IsNullOrEmpty(charsetName))
                throw new ArgumentNullException(nameof(charsetName));

            return charsetName.ToLower() == SHIFT_JIS.ToLower();
        }

        public static bool IsCJK(string charsetName)
        {
            if (string.IsNullOrEmpty(charsetName))
                throw new ArgumentNullException(nameof(charsetName));

            return cjkCharsetNames.Contains(charsetName.ToLower());
        }
    }
}
