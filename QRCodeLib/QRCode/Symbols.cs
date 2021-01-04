using System;
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

        readonly int _maxVersion;
        readonly ErrorCorrectionLevel _errorCorrectionLevel;
        readonly bool _structuredAppendAllowed;
        readonly Encoding _byteModeEncoding;
        readonly Encoding _shiftJISEncoding;

        int _parity;
        Symbol _currSymbol;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="ecLevel">誤り訂正レベル</param>
        /// <param name="maxVersion">型番の上限</param>
        /// <param name="allowStructuredAppend">複数シンボルへの分割を許可するには true を指定します。</param>
        /// <param name="byteModeEncoding">バイトモードの文字エンコーディング</param>
        public Symbols(ErrorCorrectionLevel ecLevel = ErrorCorrectionLevel.M,
                       int maxVersion = Constants.MAX_VERSION,
                       bool allowStructuredAppend = false, 
                       string byteModeEncoding = "shift_jis")
        {
            if (!(Constants.MIN_VERSION <= maxVersion && maxVersion <= Constants.MAX_VERSION))
                throw new ArgumentOutOfRangeException(nameof(maxVersion));

            _items = new List<Symbol>();

            _minVersion = Constants.MIN_VERSION;

            _maxVersion                 = maxVersion;
            _errorCorrectionLevel       = ecLevel;
            _structuredAppendAllowed    = allowStructuredAppend;
            _byteModeEncoding           = Encoding.GetEncoding(byteModeEncoding);
            _shiftJISEncoding           = Encoding.GetEncoding("shift_jis");

            _parity = 0;
            _currSymbol = new Symbol(this);

            _items.Add(_currSymbol);
        }

        /// <summary>
        /// インデックス番号を指定してSymbolオブジェクトを取得します。
        /// </summary>
        public Symbol this[int index] => _items[index];

        /// <summary>
        /// シンボル数を取得します。
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// 型番の下限を取得または設定します。
        /// </summary>
        internal int MinVersion
        {
            get => _minVersion;
            set => _minVersion = value;
        }

        /// <summary>
        /// 型番の上限を取得します。
        /// </summary>
        internal int MaxVersion => _maxVersion;

        /// <summary>
        /// 誤り訂正レベルを取得します。
        /// </summary>
        internal ErrorCorrectionLevel ErrorCorrectionLevel => _errorCorrectionLevel;

        /// <summary>
        /// 構造的連接モードの使用可否を取得します。
        /// </summary>
        internal bool StructuredAppendAllowed => _structuredAppendAllowed;

        /// <summary>
        /// 構造的連接のパリティを取得します。
        /// </summary>
        internal int Parity => _parity;

        /// <summary>
        /// バイトモードの文字エンコーディングを取得します。
        /// </summary>
        internal Encoding ByteModeEncoding => _byteModeEncoding;

        /// <summary>
        /// シンボルを追加します。
        /// </summary>
        private Symbol Add()
        {
            _currSymbol = new Symbol(this);
            _items.Add(_currSymbol);
            return _currSymbol;
        }

        /// <summary>
        /// 文字列を追加します。
        /// </summary>
        public void AppendText(string s)
        {
            if (string.IsNullOrEmpty(s))
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
                        newMode = SelectModeWhileInNumeric(s, i);
                        break;
                    case EncodingMode.ALPHA_NUMERIC:
                        newMode = SelectModeWhileInAlphanumeric(s, i);
                        break;
                    case EncodingMode.EIGHT_BIT_BYTE:
                        newMode = SelectModeWhileInByte(s, i);
                        break;
                    case EncodingMode.KANJI:
                        newMode = SelectInitialMode(s, i);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                char c = s[i];

                if (newMode != oldMode)
                {
                    if (!_currSymbol.TrySetEncodingMode(newMode, c))
                    {
                        if (!_structuredAppendAllowed || _items.Count == 16)
                            throw new ArgumentException("String too long", nameof(s));

                        Add();
                        newMode = SelectInitialMode(s, i);
                        _currSymbol.TrySetEncodingMode(newMode, c);
                    }
                }

                if (!_currSymbol.TryAppend(c))
                {
                    if (!_structuredAppendAllowed || _items.Count == 16)
                        throw new ArgumentException("String too long", nameof(s));

                    Add();
                    newMode = SelectInitialMode(s, i);
                    _currSymbol.TrySetEncodingMode(newMode, c);
                    _currSymbol.TryAppend(c);
                }
            }
        }

        /// <summary>
        /// 初期モードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="start">評価を開始する位置</param>
        private EncodingMode SelectInitialMode(string s, int start)
        {           
            if (KanjiEncoder.InSubset(s[start]))
                return EncodingMode.KANJI;

            if (ByteEncoder.InExclusiveSubset(s[start]))
                return EncodingMode.EIGHT_BIT_BYTE;

            if (AlphanumericEncoder.InExclusiveSubset(s[start]))
                return SelectModeWhenInitialDataAlphanumeric(s, start);

            if (NumericEncoder.InSubset(s[start]))
                return SelectModeWhenInitialDataNumeric(s, start);

            throw new InvalidOperationException();
        }

        private EncodingMode SelectModeWhenInitialDataAlphanumeric(string s, int start)
        {
            int cnt = 0;

            for (int i = start; i < s.Length; ++i)
            {
                if (AlphanumericEncoder.InExclusiveSubset(s[i]))
                    cnt++;
                else
                    break;
            }

            int version = _currSymbol.Version;
            bool flg;

            if (1 <= version && version <= 9)
                flg = cnt < 6;
            else if (10 <= version && version <= 26)
                flg = cnt < 7;
            else if (27 <= version && version <= 40)
                flg = cnt < 8;
            else
                throw new InvalidOperationException();

            if (flg)
            {
                if ((start + cnt) < s.Length)
                {
                    if (ByteEncoder.InSubset(s[start + cnt]))
                        return EncodingMode.EIGHT_BIT_BYTE;
                }
            }

            return EncodingMode.ALPHA_NUMERIC;
        }

        private EncodingMode SelectModeWhenInitialDataNumeric(string s, int start)
        {
            int cnt = 0;

            for (int i = start; i < s.Length; ++i)
            {
                if (NumericEncoder.InSubset(s[i]))
                    cnt++;
                else
                    break;
            }

            int version = _currSymbol.Version;
            bool flg;

            if (1 <= version && version <= 9)
                flg = cnt < 4;
            else if (10 <= version && version <= 26)
                flg = cnt < 4;
            else if (27 <= version && version <= 40)
                flg = cnt < 5;
            else
                throw new InvalidOperationException();

            if (flg)
            {
                if ((start + cnt) < s.Length)
                { 
                    if (ByteEncoder.InExclusiveSubset(s[start + cnt]))
                        return EncodingMode.EIGHT_BIT_BYTE;
                }
            }

            if (1 <= version && version <= 9)
                flg = cnt < 7;
            else if (10 <= version && version <= 26)
                flg = cnt < 8;
            else if (27 <= version && version <= 40)
                flg = cnt < 9;
            else
                throw new InvalidOperationException();

            if (flg)
            {
                if ((start + cnt) < s.Length)
                { 
                    if (AlphanumericEncoder.InExclusiveSubset(s[start + cnt]))
                        return EncodingMode.ALPHA_NUMERIC;
                }
            }

            return EncodingMode.NUMERIC;
        }

        /// <summary>
        /// 数字モードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="start">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInNumeric(string s, int start)
        {
            if (KanjiEncoder.InSubset(s[start]))
                return EncodingMode.KANJI;

            if (ByteEncoder.InExclusiveSubset(s[start]))
                return EncodingMode.EIGHT_BIT_BYTE;

            if (AlphanumericEncoder.InExclusiveSubset(s[start]))
                return EncodingMode.ALPHA_NUMERIC;

            return EncodingMode.NUMERIC;
        }

        /// <summary>
        /// 英数字モードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="start">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInAlphanumeric(string s, int start)
        {
            if (KanjiEncoder.InSubset(s[start]))
                return EncodingMode.KANJI;

            if (ByteEncoder.InExclusiveSubset(s[start]))
                return EncodingMode.EIGHT_BIT_BYTE;

            if (MustChangeAlphanumericToNumeric(s, start))
                return EncodingMode.NUMERIC;

            return EncodingMode.ALPHA_NUMERIC;
        }

        private bool MustChangeAlphanumericToNumeric(string s, int start)
        {
            bool ret = false;
            int cnt = 0;

            for (int i = start; i < s.Length; ++i)
            {
                if (!AlphanumericEncoder.InSubset(s[i]))
                    break;

                if (NumericEncoder.InSubset(s[i]))
                    cnt++;
                else
                {
                    ret = true;
                    break;
                }
            }

            if (ret)
            {
                int version = _currSymbol.Version;

                if (1 <= version && version <= 9)
                    ret = cnt >= 13;
                else if (10 <= version && version <= 26)
                    ret = cnt >= 15;
                else if (27 <= version && version <= 40)
                    ret = cnt >= 17;
                else
                    throw new InvalidOperationException();
            }

            return ret;
        }

        /// <summary>
        /// バイトモードから切り替えるモードを決定します。
        /// </summary>
        /// <param name="s">対象文字列</param>
        /// <param name="start">評価を開始する位置</param>
        private EncodingMode SelectModeWhileInByte(string s, int start)
        {
            if (KanjiEncoder.InSubset(s[start]))
                return EncodingMode.KANJI;

            if (MustChangeByteToNumeric(s, start))
                return EncodingMode.NUMERIC;

            if (MustChangeByteToAlphanumeric(s, start))
                return EncodingMode.ALPHA_NUMERIC;

            return EncodingMode.EIGHT_BIT_BYTE;
        }

        private bool MustChangeByteToNumeric(string s, int start)
        {
            bool ret = false;
            int cnt = 0;

            for (int i = start; i < s.Length; ++i)
            {
                if (!ByteEncoder.InSubset(s[i]))
                    break;

                if (NumericEncoder.InSubset(s[i]))
                    cnt++;
                else if (ByteEncoder.InExclusiveSubset(s[i]))
                {
                    ret = true;
                    break;
                }
                else
                    break;
            }

            if (ret)
            {
                int version = _currSymbol.Version;

                if (1 <= version && version <= 9)
                    ret = cnt >= 6;
                else if (10 <= version && version <= 26)
                    ret = cnt >= 8;
                else if (27 <= version && version <= 40)
                    ret = cnt >= 9;
                else
                    throw new InvalidOperationException();
            }

            return ret;
        }

        private bool MustChangeByteToAlphanumeric(string s, int start)
        {
            bool ret = false;
            int cnt = 0;

            for (int i = start; i < s.Length; ++i)
            {
                if (!ByteEncoder.InSubset(s[i]))
                    break;

                if (AlphanumericEncoder.InExclusiveSubset(s[i]))
                    cnt++;
                else if (ByteEncoder.InExclusiveSubset(s[i]))
                {
                    ret = true;
                    break;
                }
                else
                    break;
            }

            if (ret)
            {
                int version = _currSymbol.Version;

                if (1 <= version && version <= 9)
                    ret = cnt >= 11;
                else if (10 <= version && version <= 26)
                    ret = cnt >= 15;
                else if (27 <= version && version <= 40)
                    ret = cnt >= 16;
                else
                    throw new InvalidOperationException();
            }
            return ret;
        }

        /// <summary>
        /// 構造的連接のパリティを更新します。
        /// </summary>
        /// <param name="c">パリティ計算対象の文字</param>
        internal void UpdateParity(char c)
        {
            byte[] charBytes;
            if (KanjiEncoder.InSubset(c))
                charBytes = _shiftJISEncoding.GetBytes(c.ToString());
            else
                charBytes = _byteModeEncoding.GetBytes(c.ToString());

            foreach (byte value in charBytes)
                _parity ^= value;
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
