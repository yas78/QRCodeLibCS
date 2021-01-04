using System;
using System.Text.RegularExpressions;

namespace Ys.Image
{
    internal class ColorCode
    {
        public const string BLACK = "#000000"; 
        public const string WHITE = "#FFFFFF";

        public static bool IsWebColor(string arg)
        {   
            bool ret = Regex.IsMatch(arg, "^#[0-9A-Fa-f]{6}$");
            return ret;
        }
    }
}
