using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportCvsLibrary
{
    public static class ExtractCandidateName
    {

        public static string GetName(string cvText)
        {
            if (string.IsNullOrWhiteSpace(cvText))
                return string.Empty;

            var cleaned = Regex.Replace(cvText, @"[\u200E\u200F\u202A-\u202E]", "").Trim().Trim('"');

            var isHebrew = Regex.IsMatch(cleaned.Substring(0, Math.Min(200, cleaned.Length)), @"[\u05D0-\u05EA]");

            return isHebrew
                ? ExtractHebrewName(cleaned)
                : ExtractEnglishName(cleaned);
        }

        private static string ExtractHebrewName(string text)
        {
            // Find the earliest anchor: email, phone, birth year, or pipe separator
            var anchors = new List<int>();

            // Email
            var emailMatch = Regex.Match(text, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            if (emailMatch.Success) anchors.Add(emailMatch.Index);

            // Phone (Israeli formats)
            var phoneMatch = Regex.Match(text, @"0\d[\d\-]{7,}");
            if (phoneMatch.Success) anchors.Add(phoneMatch.Index);

            // Birth year (שנת לידה or 4-digit year 19xx/20xx)
            var yearMatch = Regex.Match(text, @"(שנת|לידה|19\d{2}|20[0-2]\d)");
            if (yearMatch.Success) anchors.Add(yearMatch.Index);

            // Pipe separator
            var pipeMatch = Regex.Match(text, @"\|");
            if (pipeMatch.Success) anchors.Add(pipeMatch.Index);

            if (!anchors.Any())
                return string.Empty;

            // Take text before the earliest anchor
            var beforeAnchor = text.Substring(0, anchors.Min()).Trim();

            if (string.IsNullOrWhiteSpace(beforeAnchor))
                return string.Empty;

            // Extract Hebrew words only
            var hebrewOnly = Regex.Replace(beforeAnchor, @"[a-zA-Z0-9]", " ");
            var noSymbols = Regex.Replace(hebrewOnly, @"[^\u05D0-\u05EA\s\-\u05F3\u05F4]", " ");
            var collapsed = Regex.Replace(noSymbols, @"\s+", " ").Trim();

            var words = collapsed
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => Regex.IsMatch(w, @"^[\u05D0-\u05EA\-]+$") && w.Length >= 2 && w.Length <= 15)
                .ToList();

            if (words.Count < 2)
                return string.Empty;

            // Take last 2-3 words (handles RTL reversal)
            var nameWords = words.TakeLast(Math.Min(3, words.Count)).ToList();
            nameWords.Reverse();
            return string.Join(" ", nameWords);
        }

        private static string ExtractEnglishName(string text)
        {
            // Remove phone numbers
            var noPhone = Regex.Replace(text, @"[\d\-\+\(\)\|]{5,}", " ");
            var collapsed = Regex.Replace(noPhone, @"\s+", " ").Trim();
            var words = collapsed.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Manager", "Engineer", "Developer", "Director", "Lead", "Senior", "Junior",
        "Technical", "Customer", "Success", "Operations", "Summary", "Experience",
        "Education", "Skills", "Profile", "Objective", "Overview", "Head", "Chief",
        "Officer", "Analyst", "Consultant", "Specialist", "Associate", "Assistant",
        "Vice", "President", "Executive", "Coordinator", "Administrator", "Architect",
        "Designer", "Programmer", "Intern", "Graduate", "Professional", "Global",
        "Regional", "National", "Group", "Team", "Project", "Product", "Sales",
        "Marketing", "Finance", "HR", "IT", "CEO", "CTO", "CFO", "COO", "VP"
    };

            for (int i = 0; i < words.Count - 1; i++)
            {
                var w1 = words[i];
                var w2 = i + 1 < words.Count ? words[i + 1] : null;
                var w3 = i + 2 < words.Count ? words[i + 2] : null;

                if (!IsTitleCaseName(w1) || stopWords.Contains(w1)) continue;
                if (w2 == null || !IsTitleCaseName(w2) || stopWords.Contains(w2)) continue;

                if (w3 != null && IsTitleCaseName(w3) && !stopWords.Contains(w3))
                    return $"{w1} {w2} {w3}";

                return $"{w1} {w2}";
            }

            return string.Empty;
        }

        private static bool IsTitleCaseName(string word)
        {
            return Regex.IsMatch(word, @"^[A-Z][a-z\-]{1,19}$");
        }

        public static (string FirstName, string LastName) SplitName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return parts.Length switch
            {
                0 => (string.Empty, string.Empty),
                1 => (parts[0], string.Empty),
                _ => (parts[0], string.Join(" ", parts[1..]))  // everything after first word
            };
        }

    }
}
