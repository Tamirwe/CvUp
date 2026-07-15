using Docnet.Core;
using Docnet.Core.Models;
using GeneralLibrary;
using Spire.Doc;
using Spire.Doc.Collections;
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
        public static string ExtractPdfTextBySpire(string fileNamePath)
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

            cvTxt = StringMethods.RemovePdfUnicodeBidirectionalChars(cvTxt);

            //if (IsTextReversed(cvTxt))
            //    cvTxt = FixReversedText(cvTxt);

            return cvTxt;
        }

        public static string ExtractPdfTextByDocnetCore(string fileNamePath)
        {
            var sb = new StringBuilder();

            using var docReader = DocLib.Instance.GetDocReader(fileNamePath, new PageDimensions(1080, 1920));

            var pageCount = docReader.GetPageCount();

            for (int i = 0; i < pageCount; i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                sb.Append(pageReader.GetText());
            }

            string cvTxt = sb.ToString();

            cvTxt = StringMethods.RemovePdfUnicodeBidirectionalChars(cvTxt);

            string textLanguage = StringMethods.DetectStringLanguage(cvTxt);

            if (textLanguage != "English" && StringMethods.IsLikelyReversedHebrew(cvTxt))
                cvTxt = StringMethods.ReverseHebrewText(cvTxt);

            return cvTxt;

        }


        #endregion

        #region WORD Extract Text

        public static string ExtractWordText(string fileNamePath)
        {
            var document = new Spire.Doc.Document();
            document.LoadFromFile(fileNamePath);

            var text = document.GetText();

            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        //public static string ExtractWordText(string fileNamePath)
        //{
        //    var document = new Spire.Doc.Document();
        //    document.LoadFromFile(fileNamePath);

        //    var sb = new StringBuilder();

        //    foreach (Section section in document.Sections)
        //    {
        //        // Header first, so it reads in document order
        //        var header = section.HeadersFooters.Header;
        //        if (header != null)
        //        {
        //            AppendChildObjectsText(header.ChildObjects, sb);
        //        }

        //        AppendChildObjectsText(section.Body.ChildObjects, sb);
        //    }

        //    return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
        //}

        //private static void AppendChildObjectsText(DocumentObjectCollection childObjects, StringBuilder sb)
        //{
        //    foreach (DocumentObject obj in childObjects)
        //    {
        //        if (obj is Paragraph paragraph)
        //        {
        //            var paraText = ExtractParagraphText(paragraph);
        //            if (!string.IsNullOrWhiteSpace(paraText))
        //                sb.Append(paraText.Trim()).Append(" ");
        //        }
        //        else if (obj is Table table)
        //        {
        //            foreach (TableRow row in table.Rows)
        //            {
        //                foreach (TableCell cell in row.Cells)
        //                {
        //                    foreach (Paragraph cellPara in cell.Paragraphs)
        //                    {
        //                        var cellText = ExtractParagraphText(cellPara);
        //                        if (!string.IsNullOrWhiteSpace(cellText))
        //                            sb.Append(cellText.Trim()).Append(" ");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        //private static string ExtractParagraphText(Paragraph paragraph)
        //{
        //    var sb = new StringBuilder();

        //    foreach (DocumentObject child in paragraph.ChildObjects)
        //    {
        //        if (child is TextRange textRange)
        //        {
        //            sb.Append(textRange.Text).Append(" ");
        //        }
        //        else if (child is Break)
        //        {
        //            sb.Append(" ");
        //        }
        //    }

        //    return sb.ToString();
        //}

        #endregion

     

    }
}
