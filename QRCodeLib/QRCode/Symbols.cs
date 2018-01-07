using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Ys.QRCode.Encoder;

namespace Ys.QRCode
{
    /// <summary>
    /// シンボルのコレクションを表します。
    /// </summary>
    public class Symbols : IEnumerable<Symbol>  
    {
        readonly List<Symbol> _items;

        int _minVersion;

        readonly ErrorCorrectionLevel _errorCorrectionLevel;
        readonly int                  _maxVersion;
        readonly bool                 _structuredAppendAllowed;
        readonly Encoding             _byteModeEncoding;

        int _structuredAppendParity;
        Symbol _currSymbol;
        
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="maxVersion">型番の上限</param>
        /// <param name="allowStructuredAppend">複数シンボルへの分割を許可するには true を指定します。</param>
        /// <param name="byteModeEncoding">バイトモードの文字エンコーディング</param>
        public Symbols(ErrorCorrectionLevel ecLevel = ErrorCorrectionLevel.M,
                       int maxVersion = 40,
                       bool allowStructuredAppend = false, 
                       string byteModeEncoding = "shift_jis")
        {
            if (maxVersion < Constants.MIN_VERSION || 
                maxVersion > Constants.MAX_VERSION)
                throw new ArgumentOutOfRangeException(nameof(maxVersion));

            _items = new List<Symbol>();

            _minVersion = 1;

            _errorCorrectionLevel       = ecLevel;
            _maxVersion                 = maxVersion;
            _structuredAppendAllowed    = allowStructuredAppend;
            _byteModeEncoding           = Encoding.GetEncoding(byteModeEncoding);
            
            _structuredAppendParity = 0;
            _currSymbol = new Symbol(this);

            _items.Add(_currSymbol);
        }
        
        /// <summary>
        /// インデックス番号を指定してSymbolオブジェクトを取得します。
        /// </summary>
        public Symbol this[int index]
        {
            get { return _items[index]; }
        }

        /// <summary>
        /// シンボル数を取得します。
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// 型番の下限を取得または設定します。
        /// </summary>
        internal int MinVersion
        {
            get { return _minVersion; }
            set { _minVersion = value; }
        }

        /// <summary>
        /// 型番の上限を取得します。
        /// </summary>
        internal int MaxVersion
        {
            get { return _maxVersion; }
        }

        /// <summary>
        /// 誤り訂正レベルを取得します。
        /// </summary>
        internal ErrorCorrectionLevel ErrorCorrectionLevel
        {
            get { return _errorCorrectionLevel; }
        }
        
        /// <summary>
        /// 構造的連接モードの使用可否を取得します。
        /// </summary>
        internal bool StructuredAppendAllowed
        {
            get { return _structuredAppendAllowed; }
        }

        /// <summary>
        /// 構造的連接のパリティを取得します。
        /// </summary>
        internal int StructuredAppendParity
        {
            get { return _structuredAppendParity; }
        }
        
        /// <summary>
        /// バイトモードの文字エンコーディングを取得します。
        /// </summary>
        internal Encoding ByteModeEncoding
        {
            get { return _byteModeEncoding; }
        }

        /// <summary>
        /// シンボルを追加します。
        /// </summary>
        private Symbol Add()
        {
            Debug.Assert(_structuredAppendAllowed);
            Debug.Assert(_items.Count < 16);

            _currSymbol = new Symbol(this);
            _items.Add(_currSymbol);
            return _currSymbol;
        }

        /// <summary>
        /// 文字列を追加します。
        /// </summary>
        public void AppendString(string s)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException(nameof(s));

            for (int i = 0; i < s.Length; ++i)
            {
                EncodingMode oldMode = _currSymbol.CurrentEncodingMode;
                EncodingMode newMode;

                switch (oldMode)
                {
                    case EncodingMode.UNKNOWN:
                        newMode = SelectInitialMode(s, i);
                        break;
                    case EncodingMode.NUMERIC:
                        newMode = SelectModeWhileInNumericMode(s, i);
                        break;
                    case EncodingMode.ALPHA_NUMERIC:
                        newMode = SelectModeWhileInAlphanumericMode(s, i);
                        break;
                    case EncodingMode.EIGHT_BIT_BYTE:
                        newMode = SelectModeWhileInByteMode(s, i);
                        break;
                    case EncodingMode.KANJI:
                        newMode = SelectInitialMode(s, i);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (newMode != oldMode)
                {
                    if (!_currSymbol.TrySetEncodingMode(newMode, s[i]))
                    {
                        if (!_structuredAppendAllowed || _items.Count == 16)
                            throw new ArgumentException("String too long", nameof(s));

                        Add();
                        newMode = SelectInitialMode(s, i);
                        _currSymbol.TrySetEncodingMode(newMode, s[i]);
                    }
                }

                if (!_currSymbol.TryAppend(s[i]))
                {
                    if (!_structuredAppendAllowed || _items.Count == 16)
                        throw new ArgumentException("String too long", nameof(s));

                    Add();
                    newMode = SelectInitialMode(s, i);
                    _currSymbol.TrySetEncodingMode(newMode, s[i]);
                    _currSymbol.TryAppend(s[i]);
                }
            }
        }

        /// <summary>
        /// 初期モードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="startIndex">評価を開始する位置</param>
        private EncodingMode SelectInitialMode(string s, int startIndex)
        {
            int version = _currSymbol.Version;
            
            if (KanjiEncoder.IsInSubset(s[startIndex]))
                return EncodingMode.KANJI;

            if (ByteEncoder.IsInExclusiveSubset(s[startIndex]))
                return EncodingMode.EIGHT_BIT_BYTE;

            if (AlphanumericEncoder.IsInExclusiveSubset(s[startIndex]))
            {
                int cnt = 0;
                bool flg = false;

                for (int i = startIndex; i < s.Length; ++i)
                {
                    if (AlphanumericEncoder.IsInExclusiveSubset(s[i]))
                        cnt++;
                    else
                        break;
                }

                if (version >= 1 && version <= 9)
                    flg = cnt < 6;
                else if (version >= 10 && version <= 26)
                    flg = cnt < 7;
                else if (version >= 27 && version <= 40)
                    flg = cnt < 8;
                else
                    throw new InvalidOperationException();

                if (flg)
                {
                    if ((startIndex + cnt) < s.Length)
                    {
                        if (ByteEncoder.IsInExclusiveSubset(s[startIndex + cnt]))
                            return EncodingMode.EIGHT_BIT_BYTE;
                        else
                            return EncodingMode.ALPHA_NUMERIC;
                    }
                    else
                        return EncodingMode.ALPHA_NUMERIC;
                }
                else
                    return EncodingMode.ALPHA_NUMERIC;
            }

            if (NumericEncoder.IsInSubset(s[startIndex]))
            {
                int cnt = 0;
                bool flg1 = false;
                bool flg2 = false;

                for (int i = startIndex; i < s.Length; ++i)
                {
                    if (NumericEncoder.IsInSubset(s[i]))
                        cnt++;
                    else
                        break;
                }

                if (version >= 1 && version <= 9)
                {
                    flg1 = cnt < 4;
                    flg2 = cnt < 7;
                }
                else if (version >= 10 && version <= 26)
                {
                    flg1 = cnt < 4;
                    flg2 = cnt < 8;
                }
                else if (version >= 27 && version <= 40)
                {
                    flg1 = cnt < 5;
                    flg2 = cnt < 9;
                }
                else
                    throw new InvalidOperationException();

                if (flg1)
                {
                    if ((startIndex + cnt) < s.Length)
                        flg1 = ByteEncoder.IsInExclusiveSubset(s[startIndex + cnt]);
                    else
                        flg1 = false;
                }

                if (flg2)
                {
                    if ((startIndex + cnt) < s.Length)
                        flg2 = AlphanumericEncoder.IsInExclusiveSubset(s[startIndex + cnt]);
                    else
                        flg2 = false;
                }

                if (flg1)
                    return EncodingMode.EIGHT_BIT_BYTE;
                else if (flg2)
                    return EncodingMode.ALPHA_NUMERIC;
                else
                    return EncodingMode.NUMERIC;
            }
            throw new InvalidOperationException();
        }
        
        /// <summary>
        /// 数字モードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="startIndex">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInNumericMode(string s, int startIndex)
        {
            if (ByteEncoder.IsInExclusiveSubset(s[startIndex]))
                return EncodingMode.EIGHT_BIT_BYTE;

            if (KanjiEncoder.IsInSubset(s[startIndex]))
                return EncodingMode.KANJI;

            if (AlphanumericEncoder.IsInExclusiveSubset(s[startIndex]))
                return EncodingMode.ALPHA_NUMERIC;
            
            return EncodingMode.NUMERIC;
        }

        /// <summary>
        /// 英数字モードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="startIndex">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInAlphanumericMode(string s, int startIndex)
        {
            int version = _currSymbol.Version;

            if (KanjiEncoder.IsInSubset(s[startIndex]))
                return EncodingMode.KANJI;

            if (ByteEncoder.IsInExclusiveSubset(s[startIndex]))
                return EncodingMode.EIGHT_BIT_BYTE;

            int cnt = 0;
            bool flg = false;

            for (int i = startIndex; i < s.Length; ++i)
            {
                if (!AlphanumericEncoder.IsInSubset(s[i]))
                    break;

                if (NumericEncoder.IsInSubset(s[i]))
                    cnt++;
                else
                {
                    flg = true;
                    break;
                }
            }

            if (flg)
            {
                if (version >= 1 && version <= 9)
                    flg = cnt >= 13;
                else if (version >= 10 && version <= 26)
                    flg = cnt >= 15;
                else if (version >= 27 && version <= 40)
                    flg = cnt >= 17;
                else
                    throw new InvalidOperationException();

                if (flg)
                    return EncodingMode.NUMERIC;
            }

            return EncodingMode.ALPHA_NUMERIC;
        }

        /// <summary>
        /// バイトモードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="startIndex">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInByteMode(string s, int startIndex)
        {
            int version = _currSymbol.Version;

            int cnt = 0;
            bool flg = false;
            
            if (KanjiEncoder.IsInSubset(s[startIndex]))
                return EncodingMode.KANJI;

            for (int i = startIndex; i < s.Length; ++i)
            {
                if (!ByteEncoder.IsInSubset(s[i]))
                    break;

                if (NumericEncoder.IsInSubset(s[i]))
                    cnt++;
                else if (ByteEncoder.IsInExclusiveSubset(s[i]))
                {
                    flg = true;
                    break;
                }
                else
                    break;
            }

            if (flg)
            {
                if (version >= 1 && version <= 9)
                    flg = cnt >= 6;
                else if (version >= 10 && version <= 26)
                    flg = cnt >= 8;
                else if (version >= 27 && version <= 40)
                    flg = cnt >= 9;
                else
                    throw new InvalidOperationException();

                if (flg)
                    return EncodingMode.NUMERIC;
            }

            cnt = 0;
            flg = false;

            for (int i = startIndex; i < s.Length; ++i)
            {
                if (!ByteEncoder.IsInSubset(s[i]))
                    break;

                if (AlphanumericEncoder.IsInExclusiveSubset(s[i]))
                    cnt++;
                else if (ByteEncoder.IsInExclusiveSubset(s[i]))
                {
                    flg = true;
                    break;
                }
                else
                    break;
            }

            if (flg)
            {
                if (version >= 1 && version <= 9)
                    flg = cnt >= 11;
                else if (version >= 10 && version <= 26)
                    flg = cnt >= 15;
                else if (version >= 27 && version <= 40)
                    flg = cnt >= 16;
                else
                    throw new InvalidOperationException();

                if (flg)
                    return EncodingMode.ALPHA_NUMERIC;
            }

            return EncodingMode.EIGHT_BIT_BYTE;
        }

        /// <summary>
        /// 構造的連接のパリティを更新します。
        /// </summary>
        /// <param name="c">パリティ計算対象の文字</param>
        internal void UpdateParity(char c)
        {
            byte[] charBytes = _byteModeEncoding.GetBytes(c.ToString());
            
            foreach (byte value in charBytes)
                _structuredAppendParity ^= value;
        }

        #region IEnumerable<Symbols.Symbol> Implementation
        public IEnumerator<Symbol> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
