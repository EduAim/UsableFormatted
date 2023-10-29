using Microsoft.Office.Interop.Word;
using UsfoModels;

namespace Usfo
{
    public class InteropEngine : UsfoEngine
    {
        private bool? _isEngineAvailable = null;

        public override bool IsEngineAvailable()
        {
            if (_isEngineAvailable != null)
                return (bool)_isEngineAvailable;

            try
            {
                var word = new Application();
                _isEngineAvailable = true;
            }
            catch (Exception)
            {
                _isEngineAvailable = false;
            }
            return (bool)_isEngineAvailable;
        }

        public override bool TryOpenDocx(string docxFileName)
        {
            try
            {
                var ap = new Application();
                ap.Visible = true;
                var doc = ap.Documents.Open(docxFileName);
                doc.Activate();
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        public override (bool result, string errorMessage) CreatePdfFromDocx(string docxFileName, string pdfFileName)
        {
            try
            {
                var wordApplication = new Application()
                {
                    DisplayAlerts = WdAlertLevel.wdAlertsNone,
                };

                object oMissing = System.Reflection.Missing.Value;
                object oFalse = false;
                object filename = (object)docxFileName;

                Document doc = wordApplication.Documents.Open(ref filename, ref oMissing,
                    ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                doc.Activate();

                doc.ExportAsFixedFormat(pdfFileName, WdExportFormat.wdExportFormatPDF, false, WdExportOptimizeFor.wdExportOptimizeForOnScreen,
                    WdExportRange.wdExportAllDocument, 1, 1, WdExportItem.wdExportDocumentContent, true, true,
                    WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, ref oMissing);

                // close word doc and word app.
                object saveChanges = WdSaveOptions.wdDoNotSaveChanges;

                ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);

                ((_Application)wordApplication).Quit(ref oMissing, ref oMissing, ref oMissing);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return (false, "Kļūda izveidojot PDF (#ME009)" + Environment.NewLine + ex.Message);
            }
        }

        public override (bool result, string errorMessage) CopyFromDoc(string sourceFile, string destinationFile)
        {
            try
            {
                var word = new Application();
                var document = word.Documents.Open(sourceFile);
                document.SaveAs2(destinationFile, WdSaveFormat.wdFormatXMLDocument, CompatibilityMode: WdCompatibilityMode.wdWord2010);
                word.ActiveDocument.Close();
                word.Quit();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return (false, "Kļūda nolasot dokumentu (ME010)" + Environment.NewLine + ex.Message);
            }
        }
    }
}