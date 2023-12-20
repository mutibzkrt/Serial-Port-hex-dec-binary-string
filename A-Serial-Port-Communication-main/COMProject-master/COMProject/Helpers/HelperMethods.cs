using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMProject.Helpers
{
    public static class HelperMethods
    {
        public static bool IsHex(char c)
        {
            return ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
        }
        public static bool IsDecimal(char c)
        {
            return !(!char.IsDigit(c) && c != 8 && c != 46 && c != ',');
        }
        public static bool IsBinary(char c)
        {
            return !(c != '1' && c != '0');
        }
    }
}
