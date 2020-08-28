using System;

namespace Ys.QRCode
{
    /// <summary>
    /// 符号化モード
    /// </summary>
    internal enum EncodingMode
    {
        UNKNOWN,
        NUMERIC,        
        ALPHA_NUMERIC,
        EIGHT_BIT_BYTE,
        KANJI,
    }

    /// <summary>
    /// 誤り訂正レベル
    /// </summary>
    public enum ErrorCorrectionLevel
    {
        L,
        M,
        Q,
        H,
    }
}
