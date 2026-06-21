using System.Text;
using System.Text.RegularExpressions;

namespace GeneralLibrary
{
    public static class StringMethods
    {
       
        public static string ExtractPlainText(string text)
        {
            string textLanguage = DetectStringLanguage(text);
            string plainText = RemovePunctuationAndNormelizeHebrew(text, textLanguage);
            return plainText;
        }

        public static string DetectStringLanguage(string text)
        {
            int total = 0;
            int hebrew = 0;
            int english = 0;

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    total++;
                    if (c >= '\u05D0' && c <= '\u05EA') hebrew++;
                    else if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z') english++;
                }
            }

            if (total == 0) return "English";

            double hebrewRatio = (double)hebrew / total;
            double englishRatio = (double)english / total;

            if (hebrewRatio > 0.6) return "Hebrew";
            if (englishRatio > 0.6) return "English";
            return "Mixed";
        }

        public static string RemovePunctuationAndNormelizeHebrew(string cvTxt, string cvLanguage)
        {
            // Remove invisible bidirectional marks
            string visibleText = Regex.Replace(cvTxt, @"[\u200E\u200F\u202A-\u202E]", "");

            // Protect Hebrew abbreviations: replace " between Hebrew letters with a placeholder
            // e.g. צה"ל → צה§ל
            var protectedAbbrev = Regex.Replace(visibleText,
                @"([\u05D0-\u05EA])""([\u05D0-\u05EA])",
                "$1§$2");

            // Keep letters, digits, spaces, and characters needed for emails/tech terms + hyphen
            var onlyLettersDigitsSpaces = Regex.Replace(protectedAbbrev, @"[^\p{L}\p{N}\s#\.\+@\-§]", " ");

            // Collapse spaces
            var cleanText = Regex.Replace(onlyLettersDigitsSpaces, @"\s+", " ");

            // Restore Hebrew abbreviation quotes
            cleanText = cleanText.Replace("§", "\"");

            if (cvLanguage == "Hebrew")
            {
                if (IsLikelyReversedHebrew(cleanText))
                {
                    cleanText = ReverseHebrewText(cleanText);
                }
            }

            return cleanText;
        }

        public static string RemovePdfUnicodeBidirectionalChars(string cvTxt)
        {
            // Remove null bytes and other control characters PostgreSQL can't handle
            string txt = Regex.Replace(cvTxt, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

            // Remove Unicode bidirectional control characters
            txt = Regex.Replace(txt, @"[\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u2066\u2067\u2068\u2069]", "");

            txt = Regex.Replace(txt, @"\s+", " ");
            txt = txt.Length > 7999 ? txt.Substring(0, 7999) : txt;
            return txt;
        }

        public static bool IsLikelyReversedHebrew(string text)
        {
            string[] normalWords =
            {
        // Common words
        "של", "את", "עם", "על", "אני", "הוא", "היא", "לא", "כן", "רחוב",
        // CV domain
        "ניסיון", "ניהול", "תאריך", "כישורים", "השכלה", "תעסוקה", "עבודה",
        "תפקיד", "חברה", "פרויקט", "צוות", "תקציב", "דוחות", "הכשרה",
        "מיומנויות", "הישגים", "אחריות", "תכנון", "פיתוח", "שיווק",
        "מכירות", "לקוחות", "ספקים", "תהליכים", "אסטרטגיה", "הנהלה",
        "כספים", "שכר", "גיוס", "הדרכה", "בקרה", "תפעול", "רכש",
        // Education
        "תואר", "אוניברסיטה", "מכללה", "לימודים", "הסמכה", "קורס",
        // Time
        "שנים", "חודשים", "שנה", "נוכחי", "עד", "מאז",
        // Seniority
        "מנהל", "סמנכל", "מנכל", "ראש", "עוזר", "בכיר", "זוטר",
    };

            string[] reversedWords =
            {
        // Common words
        "לש", "תא", "םע", "לע", "ינא", "אוה", "איה", "אל", "ןכ", "בוחר",
        // CV domain
        "ןויסינ", "לוהינ", "ךיראת", "םירושיכ", "הלכשה", "הקוסעת", "הדובע",
        "דיקפת", "הרבח", "טקיורפ", "תווצ", "ביצקת", "תוחוד", "הרשכה",
        "תויונמוימ", "םיגשיה", "תוירחא", "ןונכת", "חותיפ", "קוויש",
        "תוריכמ", "תוחוקל", "םיקפס", "םיכילהת", "היגטרטסא", "הלהנה",
        "םיפסכ", "רכש", "סויג", "הכרדה", "הרקב", "לועפת", "שכר",
        // Education
        "ראות", "הטיסרבינוא", "הללכמ", "םידומיל", "הכמסה", "סרוק",
        // Time
        "םינש", "םישדוח", "הנש", "יחכונ", "דע", "זאמ",
        // Seniority
        "להנמ", "לכנמס", "לכנמ", "שאר", "רזוע", "ריכב", "רטוז",
    };

            int normalCount = normalWords.Count(w => text.Contains(w));
            int reversedCount = reversedWords.Count(w => text.Contains(w));

            return reversedCount > normalCount;
        }

        public static string ReverseHebrewText(string text)
        {
            var lines = text.Split('\n');
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                var words = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fixedWords = words.Select(w => IsNumeric(w) || w.Contains('@') ? w : ReverseString(w));
                sb.AppendLine(string.Join(" ", fixedWords));
            }

            return sb.ToString();
        }

        private static bool IsNumeric(string s)
        {
            return s.All(c => char.IsDigit(c) || c == '.' || c == '-' || c == ',' || c == '+' || c == '%');
        }

        private static string ReverseString(string s)
        {
            var chars = s.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

     
    }
}
