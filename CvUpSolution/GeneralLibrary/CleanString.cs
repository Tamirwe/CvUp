using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneralLibrary
{
    public static class CleanString
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
                    cleanText = Reverse(cleanText);
                }
            }

            return cleanText;
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

        public static string Reverse(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Split into tokens preserving whitespace
            var tokens = Regex.Split(input, @"(\s+)");

            // Reverse only the non-whitespace tokens, keep whitespace in place
            var words = tokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
            var spaces = tokens.Where(t => string.IsNullOrWhiteSpace(t)).ToArray();

            Array.Reverse(words);

            // Interleave words and spaces back together
            var sb = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                sb.Append(words[i]);
                if (i < spaces.Length)
                    sb.Append(spaces[i]);
            }

            return sb.ToString();
        }

        //public static string RemovePunctuationAndNormelizeHebrew(string cvTxt, string cvLanguage)
        //{
        //    // Remove invisible bidirectional marks
        //    string visibleText = Regex.Replace(cvTxt, @"[\u200E\u200F\u202A-\u202E]", "");

        //    // Keep letters, digits, spaces, and characters needed for emails/tech terms
        //    var onlyLettersDigitsSpaces = Regex.Replace(visibleText, @"[^\p{L}\p{N}\s#\.\+@]", " ");

        //    // Collapse spaces
        //    var cleanText = Regex.Replace(onlyLettersDigitsSpaces, @"\s+", " ");

        //    if (cvLanguage == "Hebrew")
        //    {
        //        if (IsLikelyReversedHebrew(cleanText))
        //        {
        //            cleanText = Reverse(cleanText);
        //        }
        //    }

        //    return cleanText;
        //}

        //public static string RemovePunctuationAndNormelizeHebrew(string cvTxt, string cvLanguage)
        //{
        //    // Matches invisible" bidirectional marks U+200E (LRM), U+200F (RLM), and other BiDi control chars
        //    string visibleText = Regex.Replace(cvTxt, @"[\u200E\u200F\u202A-\u202E]", "");

        //    // Remove special characters, without C#,.NET,C++.
        //    var onlyLettersDigitsSpaces = Regex.Replace(visibleText, @"[^\p{L}\p{N}\s#\.\+]", " ");

        //    // Collapse spaces
        //    var cleanText = Regex.Replace(onlyLettersDigitsSpaces, @"\s+", " ");

        //    if (cvLanguage == "Hebrew")
        //    {
        //        if (IsLikelyReversedHebrew(cleanText))
        //        {
        //            cleanText = Reverse(cleanText);
        //        }
        //    }

        //    return cleanText;
        //}




        //public static string Reverse(string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return input;

        //    //return Regex.Replace(input, @"\S+", m =>
        //    //    new string(m.Value.Reverse().ToArray()));

        //    return string.Create(input.Length, input, (chars, state) =>
        //    {
        //        state.AsSpan().CopyTo(chars);
        //        chars.Reverse();
        //    });
        //}
    }
}
