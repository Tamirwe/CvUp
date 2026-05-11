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
            // Matches invisible" bidirectional marks U+200E (LRM), U+200F (RLM), and other BiDi control chars
            string visibleText = Regex.Replace(cvTxt, @"[\u200E\u200F\u202A-\u202E]", "");

            // Remove special characters, without C#,.NET,C++.
            var onlyLettersDigitsSpaces = Regex.Replace(visibleText, @"[^\p{L}\p{N}\s#\.\+]", " ");

            // Collapse spaces
            var cleanText = Regex.Replace(onlyLettersDigitsSpaces, @"\s+", " ");

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
            if (string.IsNullOrWhiteSpace(text)) return false;

            // Split into individual words
            string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Final forms that should ONLY be at the end of a word
            char[] sofitLetters = { 'ך', 'ם', 'ן', 'ף', 'ץ' };
            // Non-final forms that should NOT be at the end of a word
            char[] nonSofitEndings = { 'כ', 'מ', 'נ', 'פ', 'צ' };

            int count = 0;

            foreach (var word in words)
            {
                // 1. Check if word starts with a "Final" letter
                if (sofitLetters.Contains(word[0]) && word.Length > 1) count++;

                if (count > 3)
                {
                    return true;
                }
            }

            return false;
        }

        public static string Reverse(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            //return Regex.Replace(input, @"\S+", m =>
            //    new string(m.Value.Reverse().ToArray()));

            return string.Create(input.Length, input, (chars, state) =>
            {
                state.AsSpan().CopyTo(chars);
                chars.Reverse();
            });
        }
    }
}
