using System;

namespace Ys.QRCode.Format
{
    /// <summary>
    /// 構造的連接
    /// </summary>
    internal static class StructuredAppend
    {
        // パリティデータのビット数
        public const int PARITY_DATA_LENGTH = 8;

        // 構造的連接ヘッダーのビット数
        public const int HEADER_LENGTH = ModeIndicator.LENGTH +
                                         SymbolSequenceIndicator.POSITION_LENGTH +
                                         SymbolSequenceIndicator.TOTAL_NUMBER_LENGTH +
                                         PARITY_DATA_LENGTH;
    }
}
