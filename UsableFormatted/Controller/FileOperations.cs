using Usfo;
using UsfoModels;
using UsfoModels.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UsableFormatted.Controller
{
    internal static class FileOperations
    {
        internal const string DP_EXTENSION = ".usfo";
        internal static readonly string[] AccceptedExtenstions = new string[]
            {
                ".docx",
                ".doc",
                ".ofd",
                ".pdf",
            };
        internal static string AutoLaunchFileName { get; set; } = string.Empty;

        private static string? _tempPath = null;
        private static string _docxPath = string.Empty;
        private static string _pdfPath = string.Empty;

        private static List<string>_filesToRemove = new List<string>();

        internal static string DocxPath
        { get { return _docxPath; } }
        internal static string PdfPath
        { get { return _pdfPath; } }

        internal static string GetTempPath()
        {
            if (_tempPath == null)
                _tempPath = Path.GetTempPath();
            return _tempPath;
        }

        internal static (bool result, bool isPdf, string errorMessage) ProcessFile(FormatSet formatSet)
        {
            var engine = UsfoController.GetEngine();
            var docResult = engine.ProcessDocument(FileOperations.DocxPath, formatSet);
            if (!docResult.result)
                return (false, false, "Dokumenta apstrādes kļūda" + Environment.NewLine + docResult.errorMessage);

            if (!engine.IsEngineAvailable())
                return (true, false, string.Empty);

            var convResult = engine.CreatePdfFromDocx(FileOperations.DocxPath, FileOperations.PdfPath);
            if (!convResult.result)
                return (false, false, "Dokumenta konvertācijas kļūda" + Environment.NewLine + convResult.errorMessage);

            return (true, true, string.Empty);
        }

        internal static (bool result, string errorMessage) UploadNewFile(string sourceFile)
        {
            try
            {
                var result = true;
                var errorMessage = string.Empty;
                var souceFileName = PrepareDoc(sourceFile);

                var shortName = string.Empty;
                if (IsFileType(souceFileName, ".docx", out shortName))
                {
                    var fileName = $"{shortName}-{DateTime.Now:yyyyMMddHHmmss}";
                    _docxPath = Path.Combine(GetTempPath(), $"{fileName}.docx");
                    _pdfPath = Path.Combine(GetTempPath(), $"{fileName}.pdf");
                    File.Copy(sourceFile, _docxPath, true);
                }
                else if (IsFileType(souceFileName, new string[] { ".doc", ".odt", ".pdf" } , out shortName))
                {
                    var fileName = Path.Combine(GetTempPath(), $"{shortName}-{DateTime.Now:yyyyMMddHHmmss}");
                    _docxPath = Path.Combine(GetTempPath(), $"{fileName}.docx");
                    _pdfPath = Path.Combine(GetTempPath(), $"{fileName}.pdf");
                    var engine = UsfoController.GetEngine();
                    (result, errorMessage) = engine.CopyFromDoc(sourceFile, _docxPath);
                    if (!result)
                        return (false, errorMessage);
                }
                else
                {
                    errorMessage = "Neatpazīts faila formāts!";
                    result = false;
                }

                return (result, errorMessage);
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                var errorMessage = "Kļūda kopējot failu" + Environment.NewLine + ex.Message;
                return (false, errorMessage);
            }
        }

        private static string PrepareDoc(string sourceFileName)
        {
            try
            {
                var currentPathFileName = Path.GetFileName(sourceFileName).ToLower();
                if (!currentPathFileName.EndsWith(DP_EXTENSION))
                    return currentPathFileName;

                var tempPath = GetTempPath();
                var currentFileName = Path.GetFileName(sourceFileName);
                var newFileName = currentFileName.Substring(0, currentPathFileName.Length - DP_EXTENSION.Length);
                var newPathFileName = Path.Combine(tempPath, newFileName);
                // Try rename
                try
                {
                    File.Move(currentPathFileName, newPathFileName);
                    _filesToRemove.Add(newPathFileName);
                    return newPathFileName;
                }
                catch (Exception) { }

                // Try copy
                var tempFileName = Path.Combine(tempPath, $"dptemp-{DateTime.Now:yyyyMMddHHmmss}-" + newFileName);
                File.Copy(sourceFileName, tempFileName, true);
                _filesToRemove.Add(sourceFileName); 
                _filesToRemove.Add(tempFileName);
                return tempFileName;
            }
            catch(Exception ex )
            {
                ex.TraceEx();
                return sourceFileName;
            }
        }

        private static bool IsFileType(string fileName, string fileType, out string shortName)
        {
            shortName = string.Empty;
            try
            {
                var typeLen = fileType.Length;
                if (fileName.EndsWith(fileType))
                {
                    shortName = fileName.Substring(0, fileName.Length - typeLen);
                    return true;
                }
                else if (fileName.EndsWith(fileType + DP_EXTENSION))
                {
                    shortName = fileName.Substring(0, fileName.Length - typeLen - DP_EXTENSION.Length);
                    return true;
                }
                return false;
            } 
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        private static bool IsFileType(string fileName, IEnumerable<string> fileTypes, out string shortName)
        {
            shortName = string.Empty;
            try
            {
                foreach(var fileType in fileTypes)
                {
                    if (IsFileType(fileName, fileType, out shortName))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        internal static void CheckAppParameters(string[] args)
        {
            //string[] args = Environment.GetCommandLineArgs();

            var output = args.Select((x, i) => $"{i}. {x}").ToList();
            output.Insert(0, DateTime.Now.ToString());
            output.Add($"Env: {Environment.CommandLine}");
            var path = Path.Combine(Path.GetTempPath(), "DP_log.txt");
            _ = File.AppendAllLinesAsync(path, output);


            if (args.Length == 0)
                return;

            for (var i = 0; i < args.Length; i++)
            {
                var fileName = args[i].ToLower();
                foreach(var extenstion in AccceptedExtenstions)
                {
                    if (fileName.EndsWith(extenstion) || fileName.EndsWith(extenstion+ DP_EXTENSION))
                    {
                        AutoLaunchFileName = fileName;
                        return;
                    }
                }
            }
        }

    }
}