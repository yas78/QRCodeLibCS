using System;
using System.Collections.Generic;
using System.Text;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// エンコーダーの基本抽象クラス
    /// </summary>
    internal abstract class QRCodeEncoder
    {
        protected List<int> _codeWords   = new List<int>();
        protected int       _charCounter = 0;
        protected int       _bitCounter  = 0;
        
        protected readonly Encoding  _encoding;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public QRCodeEncoder(Encoding encoding)
        { 
            _encoding = encoding;
        }

        /// <summary>
        /// 文字数を取得します。
        /// </summary>
        public int CharCount => _charCounter;

        /// <summary>
        /// データビット数を取得します。
        /// </summary>
        public int BitCount => _bitCounter;

        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public abstract EncodingMode EncodingMode { get; }

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public abstract int ModeIndicator { get; }

        /// <summary>
        /// 文字を追加します。
        /// </summary>
        public abstract void Append(char c);

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public abstract int GetCodewordBitLength(char c);

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public abstract byte[] GetBytes();

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public abstract bool InSubset(char c);

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public abstract bool InExclusiveSubset(char c);
    }
}
