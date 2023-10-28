using System.Diagnostics;
using UsfoModels;
using UsfoModels.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;


namespace Usfo
{
    public abstract class UsfoEngine
    {
        public abstract bool IsEngineAvailable();

        public abstract bool TryOpenDocx(string docxFileName);

        public (bool result, string errorMessage) ProcessDocument(string docxFileName, FormatSet formatSet)
        {
            try
            {
                var reqFontName = formatSet.Font;
                var setFontSize = (formatSet.FontSize * 2).ToString();
                var setHeadingFontSize = (formatSet.HeadingFontSize * 2).ToString();
                Debug.WriteLine($"ProcessDocument params: {docxFileName}, {formatSet.Font}, {formatSet.FontSize}, {setFontSize}");

                using (WordprocessingDocument processingDocument = WordprocessingDocument.Open(docxFileName, true))
                {
                    var doc = processingDocument.MainDocumentPart.Document;

                    foreach (var p in processingDocument.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                    {
                        foreach (var pPr in p.Elements<ParagraphProperties>())
                        {
                            var fontSize = pPr.Descendants<FontSize>().FirstOrDefault();
                            if (fontSize != null)
                                fontSize.Val = setFontSize;

                            var fonts = pPr.Descendants<RunFonts>().FirstOrDefault();
                            SetFont(ref fonts, reqFontName);
                        }

                        foreach (var r in p.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>())
                        {
                            var fontSize = r.Descendants<FontSize>().FirstOrDefault();
                            if (fontSize != null)
                                fontSize.Val = setFontSize;

                            var fonts = r.Descendants<RunFonts>().FirstOrDefault();
                            SetFont(ref fonts, reqFontName);

                        }
                    }

                    DocumentFormat.OpenXml.Wordprocessing.Styles s = doc.MainDocumentPart.StyleDefinitionsPart?.Styles;
                    if (s != null)
                    {
                        foreach (var wStyle in s.Elements<Style>())
                        {
                            var styleId = $"{wStyle.StyleId}";
                            var isHeading = styleId.StartsWith("Heading") || styleId == "Title";
                            var styleFontSize = isHeading ? setHeadingFontSize : setFontSize;
                            foreach (var pPr in wStyle)
                            {
                                var fontSize = pPr.Descendants<FontSize>().FirstOrDefault();
                                if (fontSize != null)
                                    fontSize.Val = styleFontSize;

                                var fonts = pPr.Descendants<RunFonts>().FirstOrDefault();
                                SetFont(ref fonts, reqFontName);
                            }
                        }

                        var defaultStyle = s.Descendants<DocDefaults>().FirstOrDefault();
                        if (defaultStyle != null)
                        {
                            foreach (var pPr in defaultStyle)
                            {
                                var fontSize = pPr.Descendants<FontSize>().FirstOrDefault();
                                if (fontSize != null)
                                    fontSize.Val = setFontSize;

                                var fonts = pPr.Descendants<RunFonts>().FirstOrDefault();
                                SetFont(ref fonts, reqFontName);
                            }
                        }
                    }

                    processingDocument.MainDocumentPart.Document.Save();
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return (false, "Kļūda apstrādājot dokumentu (#ME008)" + Environment.NewLine + ex.Message);
            }
        }

        public abstract (bool result, string errorMessage) CreatePdfFromDocx(string docxFileName, string pdfFileName);

        public abstract (bool result, string errorMessage) CopyFromDoc(string sourceFile, string destinationFile);

        private static void SetFont(ref RunFonts? fonts, string reqFontName)
        {
            if (fonts == null)
                return;

            var unchangeableFonts = new List<string>
            {
                "Wingdings",
                "Wingdings 2",
                "Wingdings 3",
                "Webdings",
            };

            if (unchangeableFonts.Contains(fonts.Ascii) || unchangeableFonts.Contains(fonts.HighAnsi))
                return;
            fonts.Ascii = reqFontName;
            fonts.HighAnsi = reqFontName;
            fonts.ComplexScript = reqFontName;
            fonts.AsciiTheme = null;
            fonts.HighAnsiTheme = null;
            fonts.ComplexScriptTheme = null;
        }
    }
}