using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLibrary
{
    public static class Utils
    {
        public static string Truncate(string value, int length)
        {
            if (value.Length > length)
                return value.Substring(0, length);
            return value;
        }
    }
}
