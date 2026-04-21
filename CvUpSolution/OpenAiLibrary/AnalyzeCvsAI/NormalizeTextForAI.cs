using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenAiLibrary.AnalyzeCvsAI
{
    internal static class NormalizeTextForAI
    {
        public static string NormalizeCvText(string cvTxt)
        {
            if (string.IsNullOrWhiteSpace(cvTxt))
                return string.Empty;

            string normalized = cvTxt.Normalize(NormalizationForm.FormC);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = char.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            normalized = sb.ToString().Normalize(NormalizationForm.FormC);

            normalized = normalized.ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^\w\s\u0590-\u05FF]", "");
            normalized = Regex.Replace(normalized, @"\s+", "").Trim();


            return normalized;
        }
    }
}
