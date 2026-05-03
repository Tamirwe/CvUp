namespace OpenAiLibrary.AnalyzeCvsAI
{

    internal static class LanguageDetector
    {
        public static string Detect(string text)
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
    }
}
