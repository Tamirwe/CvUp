using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportCvsLibrary.RegularExpressions
{
    internal static class Rx
    {
        /*
            
            [] - list of characters  ([aeiou] - Match all vowels, [\p{P}\d]	Match all punctuation and decimal digit characters)
            \p{P}    - Unicode punctuation
            (\) - indicates The character that follows it is a special character


            .	Matches one or more characters.
            ^regex	Finds regex that must match at the beginning of the line.
            regex$	Finds regex that must match at the end of the line.
            [abc]	Set definition, can match the letter a or b or c.
            [abc][vz]	Set definition, can match a or b or c followed by either v or z.
            [^abc]	When a caret appears as the first character inside square brackets, it negates the pattern. This can match any character except a or b or c.
            [a-d1-7]	Ranges: matches a letter between a and d and figures from 1 to 7.
            X|Z	Finds X or Z.
            XZ	Finds X directly followed by Z.
            $	Checks if a line end follows.
 
            \d	Any digit, short for [0-9]
            \D	A non-digit, short for [^0-9]
            \s	A whitespace character, short for [ \t\n\x0b\r\f]
            \S	A non-whitespace character, short for [^\s]
            \w	A word character, short for [a-zA-Z_0-9]
            \W	A non-word character [^\w]
            \S+	Several non-whitespace characters
            \b	Matches a word boundary where a word character is [a-zA-Z0-9_].
 
            *	Occurs zero or more times, is short for {0,}
            +	Occurs one or more times, is short for {1,}
            ?	Occurs no or one times, ? is short for {0,1}.
            {X}	Occurs X number of times, {} describes the order of the preceding liberal
            {X,Y}	Occurs between X and Y times,
            *?	? after a quantifier makes it a reluctant quantifier. It tries to find the smallest match.
            
            \s* Match zero or more white-space characters.
            [\+-]? Match zero or one occurrence of either the positive sign or the negative sign.
            \s? Match zero or one white-space character.
            \$? Match zero or one occurrence of the dollar sign.
            \s? Match zero or one white-space character.
            \d* Match zero or more decimal digits.
            \.? Match zero or one decimal point symbol.
        */
        #region RegEx Return String

        internal static string MultipleSpacesToOneSpace(string str)
        {
            return Regex.Replace(str, @"\s+", " "); //replace multiple spaces with a sigle space
        }
        internal static string RemoveEnglishLetters(string str)
        {
            return Regex.Replace(str, @"[a-zA-Z]+", String.Empty);//remove all english letters
        }
        internal static string CharsToRemove(string str, string chars)
        {
            string pt = @"[" + chars + @"]";
            return Regex.Replace(str, pt, string.Empty);//remove some charecters
            //return Regex.Replace(str, @"[*_&#^@]", string.Empty);//remove some charecters
        }
        internal static string RemoveSpacesBeforeChar(string str, string chars)
        {
            string pt = @"\s+(?=[" + chars + @"])";
            return Regex.Replace(str, pt, String.Empty);//remove spaces before char
            //return Regex.Replace(str, @"\s+(?=[:-])", String.Empty);//remove spaces before ":" and "-"
        }
        internal static string RemoveSpacesBeforeColon(string str)
        {
            return Regex.Replace(str, @"\s*:", ":"); //remove spaces before colon
        }
        internal static string RemoveSpacesBeforeAfterDash(string str)
        {
            return Regex.Replace(str, @"\s*-\s*", "-"); //remove spaces before and after dash
        }
        internal static string RemoveNoneAlfabeit(string str)
        {
            return Regex.Replace(str, @"[a-z]+", String.Empty);//remove all english letters
        }
        internal static string RemoveDigits(string str)
        {
            return Regex.Replace(str, @"[\d]", string.Empty);
        }
        internal static string RemovePunctuation(string str)
        {
            return Regex.Replace(str, @"\p{P}", string.Empty);
        }
        internal static string PunctuationTrim(string str)
        {
            return PunctuationTrimStart(PunctuationTrimEnd(str));
        }
        internal static string PunctuationTrimStart(string str)
        {
            return Regex.Replace(str, @"^\p{P}*", string.Empty);//remove punctuation at the start of a string
        }
        internal static string PunctuationTrimEnd(string str)
        {
            return Regex.Replace(str, @"\p{P}*$", string.Empty);//remove punctuation at the and of a string
        }
        internal static string TrimNoneAlfabeit(string str)
        {
            return TrimStartNoneAlfabeit(TrimEndNoneAlfabeit(str));
        }
        internal static string TrimStartNoneAlfabeit(string str)
        {
            return Regex.Replace(str, @"^[^א-ת]*", string.Empty);//remove punctuation at the start of a string
        }
        internal static string TrimEndNoneAlfabeit(string str)
        {
            return Regex.Replace(str, @"[^א-ת]*$", string.Empty);
        }
        internal static string RemoveEnglishChars(string str)
        {
            return Regex.Replace(str, @"[a-z]+", String.Empty);//remove all english letters
        }
        internal static string RemoveNoneHebChars(string str)
        {
            return Regex.Replace(str, @"[^א-ת -'\""]", String.Empty);//remove all none alfabeit charecters, exept " ","-","""
        }

        internal static string ReplaceNoneHebNameCharsToSpace(string str)
        {
            return Regex.Replace(str, @"[^א-ת '\""]", String.Empty);//remove all none alfabeit charecters, exept " ","-","""
        }
        #endregion

        #region RegEx Return Index
        internal static int FirstDigitIndex(string str, RegexOptions regexOptions = RegexOptions.RightToLeft)
        {
            if (Regex.Match(str, @"[0-9]").Success)
            {
                return Regex.Match(str, @"[0-9].+", regexOptions).Index;
            }
            return -1;
        }
        internal static int FirstCharsIndex(string str, string chars, RegexOptions regexOptions = RegexOptions.RightToLeft)
        {
            string pt = @"[" + chars + @"]";
            //return Regex.Match(str, pt, regexOptions).Index;

            if (Regex.Match(str, pt).Success)
            {
                pt = @"[" + chars + @"].+";
                return Regex.Match(str, pt, regexOptions).Index;
            }
            return -1;
        }
        #endregion

        #region RegEx Return IsMatch
        internal static bool IsHebrewCharExist(string str)
        {
            return Regex.IsMatch(str, @"[א-ת]+");
        }
        internal static bool IsHebrewCharsExist(string str)
        {
            return Regex.IsMatch(str, @"[א-ת]{2,}");
        }

        internal static bool IsSpaceExist(string str)
        {
            return Regex.IsMatch(str, @"^[ ]+$");
        }
        #endregion
    }
}