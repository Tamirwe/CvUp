using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralLibrary
{
    public static class UtilsStr
    {
        public static string? limitLen(string? original, int maxLength)
        {
            if (original != null)
            {
                return original.Substring(0, Math.Min(original.Length, maxLength));
            }
            return null;
        }
    }
}
