using UsfoModels;
using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.IO;

namespace UsableFormatted.Controller
{
    internal static class DocController
    {
        internal static bool TryOpenDocx(string docxFileName)
        {
            // Try MS Office first
            var engine = UsfoController.GetEngine();
            if (engine.IsEngineAvailable())
            {
                try
                {
                    var isOpened = engine.TryOpenDocx(docxFileName);
                    if (isOpened)
                    {
                        return true;
                    }
                }
                catch (Exception ex) 
                {
                    ex.TraceEx();
                }
            }

            try
            {
                var process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = $"start /c \"{docxFileName}\"";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = true;
                process.Start();
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static bool TryOpenPdf(string pdfFileName)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = $"start /c \"{pdfFileName}\"";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = true;
                process.Start();

                //Process.Start("cmd", $"start /c \"{pdfFileName}\"");
                return true;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal async static Task<bool> SaveDocument(string documentFileName)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Microsoft Word document (*.docx)|*.docx"
            };
            if (saveFileDialog.ShowDialog() != true)
                return false;

            _M._mainWindow.SetLoading(true);
            var isSaved = await Task.Run(async () =>
            {
                try
                {
                    var bytes = await File.ReadAllBytesAsync(documentFileName);
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, bytes);
                    return true;
                }
                catch (Exception ex)
                {
                    ex.TraceEx();
                    return false;
                }
            });
            _M._mainWindow.SetLoading(false);
            return isSaved;
        }
    }
}