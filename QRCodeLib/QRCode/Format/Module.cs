using System;
using System.Diagnostics;

namespace Ys.QRCode.Format
{
    /// <summary>
    /// モジュール
    /// </summary>
    internal static class Module
    {
        /// <summary>
        /// １辺のモジュール数を返します。
        /// </summary>
        /// <param name="version">型番</param>
        public static int GetNumModulesPerSide(int version)
        {
            return 17 + 4 * version;
        }
    }
}
