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

        public static int FileTypeKey(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".pdf":
                    return 1;
                case ".docx":
                    return 2;
                case ".doc":
                    return 3;
                default:
                    return 0;
            }
        }

        public static string FileTypeName(char key)
        {
            switch (key)
            {
                case '1':
                    return ".pdf";
                case '2':
                    return ".docx";
                case '3':
                    return ".doc";
                default:
                    return "";
            }
        }
    }
}
