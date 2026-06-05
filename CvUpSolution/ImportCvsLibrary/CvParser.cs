using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Pdf;
using Spire.Pdf.Texts;
using System.Text;
using System.Text.RegularExpressions;

namespace ImportCvsLibrary
{
    public static class CvParser
    {

        #region PDF Extract Text
        public static string ExtractPdfText(string fileNamePath)
        {
            StringBuilder cvTxtSB = new StringBuilder();

            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(fileNamePath);

            StringBuilder buffer = new StringBuilder();
            PdfTextExtractOptions extractOptions = new PdfTextExtractOptions();
            extractOptions.IsExtractAllText = true;

            foreach (PdfPageBase page in doc.Pages)
            {
                PdfTextExtractor textExtractor = new PdfTextExtractor(page);
                cvTxtSB.Append(textExtractor.ExtractText(extractOptions));
            }

            doc.Close();
            string cvTxt = cvTxtSB.ToString();

            cvTxt =  RemoveCvExtraSpaces(cvTxt);

            //if (IsTextReversed(cvTxt))
            //    cvTxt = FixReversedText(cvTxt);

            return cvTxt;
        }

        public static string RemoveCvExtraSpaces(string cvTxt)
        {
            // Remove null bytes and other control characters PostgreSQL can't handle
            string txt = Regex.Replace(cvTxt, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

            // Remove Unicode bidirectional control characters
            txt = Regex.Replace(txt, @"[\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u2066\u2067\u2068\u2069]", "");

            txt = Regex.Replace(txt, @"\s+", " ");
            txt = txt.Length > 7999 ? txt.Substring(0, 7999) : txt;
            return txt;
        }

        #endregion

        #region WORD Extract Text

        public static string ExtractWordText(string fileNamePath)
        {
            var document = new Spire.Doc.Document();
            document.LoadFromFile(fileNamePath);

            var sb = new StringBuilder();

            foreach (Section section in document.Sections)
            {
                foreach (DocumentObject obj in section.Body.ChildObjects)
                {
                    if (obj is Paragraph paragraph)
                    {
                        var paraText = ExtractParagraphText(paragraph);
                        if (!string.IsNullOrWhiteSpace(paraText))
                            sb.Append(paraText.Trim()).Append(" ");
                    }
                    else if (obj is Table table)
                    {
                        foreach (TableRow row in table.Rows)
                        {
                            foreach (TableCell cell in row.Cells)
                            {
                                foreach (Paragraph cellPara in cell.Paragraphs)
                                {
                                    var cellText = ExtractParagraphText(cellPara);
                                    if (!string.IsNullOrWhiteSpace(cellText))
                                        sb.Append(cellText.Trim()).Append(" ");
                                }
                            }
                        }
                    }
                }
            }

            return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
        }


        private static string ExtractParagraphText(Paragraph paragraph)
        {
            var sb = new StringBuilder();

            foreach (DocumentObject child in paragraph.ChildObjects)
            {
                if (child is TextRange textRange)
                {
                    sb.Append(textRange.Text).Append(" ");
                }
                else if (child is Break)
                {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Reversed Text
        private static string FixReversedText(string text)
        {
            var lines = text.Split('\n');
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                var words = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fixedWords = words.Select(w => IsNumeric(w) ? w : ReverseString(w));
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

        private static bool IsTextReversed(string text)
        {
            // These are very common Hebrew words that would appear reversed
            // if the PDF font encoding is reversed
            var reversedCommonWords = new[]
            {
        "לש",      // של
        "םע",      // עם  
        "יכ",      // כי
        "אל",      // לא - careful, also valid forward
        "ןיא",     // אין
        "ןכ",      // כן
        "וא",      // או
        "יא",      // אי
        "הז",      // זה
        "וב",      // בו
        "הב",      // בה
        "םג",      // גם
        "ךא",      // אך
        "ןמ",      // מן
        "לע",      // על
        "דע",      // עד
        "ינפל",    // לפני
        "ירחא",    // אחרי
        "ךות",     // תוך
        "יפל",     // לפי
    };

            // Count how many reversed words appear vs forward
            int reversedCount = reversedCommonWords.Count(w =>
                Regex.IsMatch(text, $@"\b{w}\b"));

            // Also count forward versions
            var forwardCommonWords = new[]
            {
        "של", "עם", "כי", "אין", "כן", "או",
        "זה", "גם", "אך", "מן", "על", "עד",
        "לפני", "אחרי", "תוך", "לפי"
    };

            int forwardCount = forwardCommonWords.Count(w =>
                Regex.IsMatch(text, $@"\b{w}\b"));

            return reversedCount > forwardCount;
        }

        #endregion

    }
}
