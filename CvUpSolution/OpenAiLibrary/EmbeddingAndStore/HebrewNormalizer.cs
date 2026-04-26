using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingAndStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;


    /// <summary>
    /// General-purpose Hebrew morphological normalizer.
    /// Reduces morphological variation so that semantically identical
    /// words produce the same token before embedding.
    ///
    /// Pipeline:
    ///   1. Strip nikud (vowel diacritics U+05B0–U+05C7)
    ///   2. Normalize final-form letters  (ך→כ  ם→מ  ן→נ  ף→פ  ץ→צ)
    ///   3. Unicode NFKC  (fancy quotes, en-dash, geresh, etc.)
    ///   4. Tokenize  (Hebrew words + ASCII tokens kept as-is)
    ///   5. Strip common attached prefixes  (ב ל מ כ ה ו ש and combos)
    ///   6. Lemmatize via dictionary  (common inflections → base form)
    ///   7. Remove stopwords
    ///   8. Deduplicate tokens  (order-preserving)
    /// </summary>
    public class HebrewTextNormalizer
    {
        // ── 1. Nikud ──────────────────────────────────────────────────────────
        private static readonly Regex NikudRegex =
            new(@"[\u05B0-\u05C7]", RegexOptions.Compiled);

        // ── 2. Final letters ──────────────────────────────────────────────────
        private static readonly Dictionary<char, char> FinalLetterMap = new()
        {
            ['ך'] = 'כ',
            ['ם'] = 'מ',
            ['ן'] = 'נ',
            ['ף'] = 'פ',
            ['ץ'] = 'צ'
        };

        // ── 3. Tokenizer: Hebrew words OR ASCII tokens ─────────────────────
        private static readonly Regex TokenRegex =
            new(@"[א-ת]+|[a-zA-Z][a-zA-Z0-9+#.\-/]*", RegexOptions.Compiled);

        // ── 4. Prefix pattern ─────────────────────────────────────────────────
        // Handles: ו?ב | ו?כ | ו?ל | ו?מ | ו?ה | וש | ש | ו
        // Only strips when remaining stem is ≥ 2 characters.
        private static readonly Regex PrefixRegex =
            new(@"^(ו?ב|ו?כ|ו?ל|ו?מ|ו?ה|וש|ש|ו)(?=[א-ת]{2,})", RegexOptions.Compiled);

        // ── 5. Lemma dictionary ───────────────────────────────────────────────
        // Maps common inflected forms → base (dictionary) form.
        // Covers verbs (past/present/future, m/f, singular/plural),
        // nouns (plural, construct state), and frequent misspellings.
        private static readonly Dictionary<string, string> LemmaDict = new()
        {
            // ── Verbs: being / having ────────────────────────────────────────
            ["היה"] = "להיות",
            ["היתה"] = "להיות",
            ["יהיה"] = "להיות",
            ["תהיה"] = "להיות",
            ["יהיו"] = "להיות",
            ["הייתי"] = "להיות",
            ["היינו"] = "להיות",

            // ── Verbs: motion ────────────────────────────────────────────────
            ["הלך"] = "ללכת",
            ["הלכה"] = "ללכת",
            ["הלכתי"] = "ללכת",
            ["הולך"] = "ללכת",
            ["הולכת"] = "ללכת",
            ["ילך"] = "ללכת",
            ["בא"] = "לבוא",
            ["באה"] = "לבוא",
            ["באתי"] = "לבוא",
            ["יבוא"] = "לבוא",
            ["בואו"] = "לבוא",
            ["יצא"] = "לצאת",
            ["יצאה"] = "לצאת",
            ["יצאתי"] = "לצאת",
            ["נכנס"] = "להיכנס",
            ["נכנסה"] = "להיכנס",

            // ── Verbs: speech / writing ──────────────────────────────────────
            ["אמר"] = "לומר",
            ["אמרה"] = "לומר",
            ["אמרתי"] = "לומר",
            ["אומר"] = "לומר",
            ["אומרת"] = "לומר",
            ["יאמר"] = "לומר",
            ["כתב"] = "לכתוב",
            ["כתבה"] = "לכתוב",
            ["כתבתי"] = "לכתוב",
            ["כותב"] = "לכתוב",
            ["כותבת"] = "לכתוב",
            ["קרא"] = "לקרוא",
            ["קראה"] = "לקרוא",
            ["קראתי"] = "לקרוא",
            ["שאל"] = "לשאול",
            ["שאלה"] = "לשאול",
            ["שאלתי"] = "לשאול",
            ["ענה"] = "לענות",
            ["ענתה"] = "לענות",
            ["עניתי"] = "לענות",

            // ── Verbs: thinking / knowing ────────────────────────────────────
            ["ידע"] = "לדעת",
            ["ידעה"] = "לדעת",
            ["ידעתי"] = "לדעת",
            ["יודע"] = "לדעת",
            ["יודעת"] = "לדעת",
            ["חשב"] = "לחשוב",
            ["חשבה"] = "לחשוב",
            ["חשבתי"] = "לחשוב",
            ["חושב"] = "לחשוב",
            ["חושבת"] = "לחשוב",
            ["הבין"] = "להבין",
            ["הבינה"] = "להבין",
            ["הבנתי"] = "להבין",
            ["מבין"] = "להבין",
            ["מבינה"] = "להבין",
            ["למד"] = "ללמוד",
            ["למדה"] = "ללמוד",
            ["למדתי"] = "ללמוד",
            ["לומד"] = "ללמוד",
            ["לומדת"] = "ללמוד",
            ["הרגיש"] = "להרגיש",
            ["הרגישה"] = "להרגיש",
            ["הרגשתי"] = "להרגיש",
            ["מרגיש"] = "להרגיש",
            ["מרגישה"] = "להרגיש",

            // ── Verbs: action ────────────────────────────────────────────────
            ["עשה"] = "לעשות",
            ["עשתה"] = "לעשות",
            ["עשיתי"] = "לעשות",
            ["עושה"] = "לעשות",
            ["עושים"] = "לעשות",
            ["נתן"] = "לתת",
            ["נתנה"] = "לתת",
            ["נתתי"] = "לתת",
            ["נותן"] = "לתת",
            ["נותנת"] = "לתת",
            ["לקח"] = "לקחת",
            ["לקחה"] = "לקחת",
            ["לקחתי"] = "לקחת",
            ["רצה"] = "לרצות",
            ["רצתה"] = "לרצות",
            ["רציתי"] = "לרצות",
            ["רוצה"] = "לרצות",
            ["רוצים"] = "לרצות",
            ["יכול"] = "יכולת",
            ["יכולה"] = "יכולת",
            ["יכולים"] = "יכולת",
            ["יכולתי"] = "יכולת",
            ["צריך"] = "צורך",
            ["צריכה"] = "צורך",
            ["צריכים"] = "צורך",
            ["צריכתי"] = "צורך",
            ["הביא"] = "להביא",
            ["הביאה"] = "להביא",
            ["הבאתי"] = "להביא",
            ["מביא"] = "להביא",
            ["שלח"] = "לשלוח",
            ["שלחה"] = "לשלוח",
            ["שלחתי"] = "לשלוח",
            ["קיבל"] = "לקבל",
            ["קיבלה"] = "לקבל",
            ["קיבלתי"] = "לקבל",
            ["מקבל"] = "לקבל",
            ["מקבלת"] = "לקבל",
            ["פתח"] = "לפתוח",
            ["פתחה"] = "לפתוח",
            ["פתחתי"] = "לפתוח",
            ["סגר"] = "לסגור",
            ["סגרה"] = "לסגור",
            ["סגרתי"] = "לסגור",
            ["ראה"] = "לראות",
            ["ראתה"] = "לראות",
            ["ראיתי"] = "לראות",
            ["רואה"] = "לראות",
            ["רואים"] = "לראות",
            ["שמע"] = "לשמוע",
            ["שמעה"] = "לשמוע",
            ["שמעתי"] = "לשמוע",
            ["שומע"] = "לשמוע",
            ["עזר"] = "לעזור",
            ["עזרה"] = "לעזור",
            ["עזרתי"] = "לעזור",
            ["עוזר"] = "לעזור",
            ["עוזרת"] = "לעזור",
            ["חיכה"] = "לחכות",
            ["חיכיתי"] = "לחכות",
            ["הגיע"] = "להגיע",
            ["הגיעה"] = "להגיע",
            ["הגעתי"] = "להגיע",
            ["מגיע"] = "להגיע",
            ["שב"] = "לשוב",
            ["שבה"] = "לשוב",
            ["שבתי"] = "לשוב",
            ["חזר"] = "לחזור",
            ["חזרה"] = "לחזור",
            ["חזרתי"] = "לחזור",
            ["התחיל"] = "להתחיל",
            ["התחילה"] = "להתחיל",
            ["התחלתי"] = "להתחיל",
            ["גמר"] = "לגמור",
            ["גמרה"] = "לגמור",
            ["גמרתי"] = "לגמור",
            ["הפסיק"] = "להפסיק",
            ["הפסיקה"] = "להפסיק",
            ["הפסקתי"] = "להפסיק",
            ["המשיך"] = "להמשיך",
            ["המשיכה"] = "להמשיך",
            ["המשכתי"] = "להמשיך",

            // ── Nouns: plural / construct ────────────────────────────────────
            ["ילדים"] = "ילד",
            ["ילדות"] = "ילדה",
            ["אנשים"] = "אדם",
            ["אנשות"] = "אדם",
            ["נשים"] = "אישה",
            ["ימים"] = "יום",
            ["שבועות"] = "שבוע",
            ["חודשים"] = "חודש",
            ["שנים"] = "שנה",
            ["שעות"] = "שעה",
            ["דקות"] = "דקה",
            ["מקומות"] = "מקום",
            ["בתים"] = "בית",
            ["ערים"] = "עיר",
            ["ארצות"] = "ארץ",
            ["דברים"] = "דבר",
            ["עניינים"] = "עניין",
            ["שאלות"] = "שאלה",
            ["תשובות"] = "תשובה",
            ["בעיות"] = "בעיה",
            ["פתרונות"] = "פתרון",
            ["רעיונות"] = "רעיון",
            ["מחשבות"] = "מחשבה",
            ["רגשות"] = "רגש",
            ["תחושות"] = "תחושה",
            ["חברים"] = "חבר",
            ["חברות"] = "חברה",
            ["עבודות"] = "עבודה",
            ["תפקידים"] = "תפקיד",
            ["כלים"] = "כלי",
            ["כלות"] = "כלי",
            ["ספרים"] = "ספר",
            ["מילים"] = "מילה",
            ["משפטים"] = "משפט",
            ["פסקאות"] = "פסקה",
            ["נושאים"] = "נושא",
            ["תחומים"] = "תחום",
            ["צבעים"] = "צבע",
            ["צורות"] = "צורה",
            ["כוחות"] = "כוח",
            ["כישורים"] = "כישור",
            ["תוצאות"] = "תוצאה",
            ["השפעות"] = "השפעה",
            ["תנאים"] = "תנאי",
            ["כללים"] = "כלל",
            ["חוקים"] = "חוק",
            ["זכויות"] = "זכות",
            ["תהליכים"] = "תהליך",
            ["שלבים"] = "שלב",
            ["מטרות"] = "מטרה",
            ["יעדים"] = "יעד",
            ["סיבות"] = "סיבה",
            ["תוצאות"] = "תוצאה",

            // ── Common misspellings ──────────────────────────────────────────
            ["נסיון"] = "ניסיון",
            ["קשיים"] = "קושי",
            ["בעייה"] = "בעיה",
            ["מידע"] = "מידע",   // keep as-is but normalise spelling variants
            ["מידע"] = "מידע",
        };

        // ── 6. Stopwords ──────────────────────────────────────────────────────
        private static readonly HashSet<string> Stopwords = new()
        {
            // Prepositions & conjunctions
            "של", "את", "עם", "על", "אל", "מן", "בין", "תוך",
            "לפי", "לפני", "אחרי", "עד", "מול", "דרך", "כמו",
            // Conjunctions
            "כי", "גם", "אם", "כן", "אבל", "אך", "ואולם", "אלא",
            "למרות", "אף", "רק", "כבר", "עוד", "שוב",
            // Pronouns
            "הוא", "היא", "הם", "הן", "אני", "אנחנו", "אנו",
            "אתה", "את", "אתם", "אתן", "זה", "זאת", "זו",
            "אלה", "אלו", "כל", "כלל",
            // Common function words
            "יש", "אין", "כן", "לא", "מה", "מי", "איך", "למה",
            "איפה", "מתי", "כמה", "הרי", "הנה", "הנה",
            // Single letters (noise)
            "א", "ב", "ג", "ד", "ה", "ו", "ז",
        };

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Normalize a Hebrew text string for embedding.
        /// </summary>
        /// <param name="text">Raw Hebrew input text.</param>
        /// <param name="removeStopwords">Drop common function words (default: true).</param>
        /// <param name="lemmatize">Reduce words to base form (default: true).</param>
        /// <param name="deduplicate">Remove repeated tokens, order-preserving (default: true).</param>
        /// <param name="minTokenLength">Drop tokens shorter than this (default: 2).</param>
        /// <returns>Normalized string ready for an embedding model.</returns>
        public string Normalize(
            string text,
            bool removeStopwords = true,
            bool lemmatize = true,
            bool deduplicate = true,
            int minTokenLength = 2)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Step 1 – strip nikud
            text = NikudRegex.Replace(text, string.Empty);

            // Step 2 – normalize final letters
            text = NormalizeFinalLetters(text);

            // Step 3 – Unicode NFKC (fancy quotes, en-dash, geresh …)
            text = text.Normalize(NormalizationForm.FormKC);

            // Step 4 – tokenize
            var tokens = TokenRegex.Matches(text).Select(m => m.Value);

            // Step 5 – per-token processing
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var raw in tokens)
            {
                // Lowercase ASCII tokens (English tech terms); Hebrew has no case
                var tok = IsAscii(raw) ? raw.ToLowerInvariant() : raw;

                if (tok.Length < minTokenLength)
                    continue;

                if (removeStopwords && Stopwords.Contains(tok))
                    continue;

                if (lemmatize && IsHebrew(tok))
                    tok = Lemmatize(tok);

                if (tok.Length < minTokenLength)
                    continue;

                if (deduplicate && !seen.Add(tok))
                    continue;

                result.Add(tok);
            }

            return string.Join(" ", result);
        }

        /// <summary>
        /// Normalize a search query.  Same pipeline as index time but stopwords
        /// are kept — they carry intent context in queries.
        /// </summary>
        public string NormalizeQuery(string query) =>
            Normalize(query, removeStopwords: false, deduplicate: false);

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        private static string NormalizeFinalLetters(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (var ch in text)
                sb.Append(FinalLetterMap.TryGetValue(ch, out var mapped) ? mapped : ch);
            return sb.ToString();
        }

        private static string Lemmatize(string word)
        {
            // 1. Direct dictionary hit
            if (LemmaDict.TryGetValue(word, out var lemma))
                return lemma;

            // 2. Strip prefix, retry in dictionary
            var stem = StripPrefix(word);
            if (stem != word && LemmaDict.TryGetValue(stem, out var stemLemma))
                return stemLemma;

            // 3. Return prefix-stripped form as best heuristic
            return stem;
        }

        private static string StripPrefix(string word)
        {
            var m = PrefixRegex.Match(word);
            if (m.Success)
            {
                var stem = word[m.Length..];
                if (stem.Length >= 2)
                    return stem;
            }
            return word;
        }

        private static bool IsHebrew(string t) =>
            t.Length > 0 && t[0] is >= '\u05D0' and <= '\u05EA';

        private static bool IsAscii(string t) =>
            t.Length > 0 && t[0] < 128;
    }
}

