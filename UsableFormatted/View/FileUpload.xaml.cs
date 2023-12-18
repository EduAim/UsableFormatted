using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsfoModels.Model;
using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsableFormatted.Repos;
using UsableFormatted.ViewModel;
using Microsoft.Win32;
using UsfoModels;
using System.Globalization;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for FileUpload.xaml
    /// </summary>
    public partial class FileUpload : UserControl
    {
        private bool AnonymousMode => UserProfileRepo.LoggedInUserId <= 0;
        private string _fileName = string.Empty;
        private bool _isMsOfficeAvailable = false;

        public FileUpload()
        {
            InitializeComponent();
            IsVisibleChanged += FileUpload_IsVisibleChanged;
            Loaded += FileUpload_Loaded;
        }

        private void FileUpload_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("FileUpload_Loaded");
            var engine = UsfoController.GetEngine();
            _isMsOfficeAvailable = engine.IsEngineAvailable();
        }

        private void FileUpload_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            ProfileBox.UpdateControl();
            _M._mainWindow.SetSurvey(false);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                FileUploadText.Text = _fileName = string.Empty;
                var dataContext = DataContext as FileUploadVM;
                if (dataContext != null)
                {
                    dataContext.IsAnonymousVisible = AnonymousMode;
                    dataContext.IsLimitedVisibility = !_isMsOfficeAvailable;
                }
                if (AnonymousMode)
                {
                    BirthYearText.Text = UserProfileRepo.AnonymousBirthYear > 0 ? UserProfileRepo.AnonymousBirthYear.ToString() : string.Empty;
                }

                var isRecentFiles = UpdateRecentFiles();
                if (dataContext != null)
                {
                    dataContext.IsHistory = isRecentFiles;
                }
                if (!string.IsNullOrEmpty(FileOperations.AutoLaunchFileName))
                    DocumentAutoLaunch();
            }));
        }

        internal async void DocumentAutoLaunch()
        {
            _fileName = FileOperations.AutoLaunchFileName;
            FileOperations.AutoLaunchFileName = string.Empty;
            _ = FileUploadText.Dispatcher.BeginInvoke(new Action(() => {
                FileUploadText.Text = _fileName;
            }));

            _M._mainWindow.SetLoading(true);
            await Task.Delay(1);
            FormatSet formatSet = AnonymousMode ? Recommendations.GetByAge(DateTime.Now.Year - 2000) : UserProfileRepo.GetUserFormatSet();

            var uploadResult = await UploadDocument();
            if (!uploadResult)
            {
                _M._mainWindow.SetLoading(false);
                return;
            }

            var processResult = await ProcessDocument(formatSet);

            _M._mainWindow.SetLoading(false);
            await Task.Delay(1);
            if (!processResult.result)
            {
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tErrorConvertingDocument"] + Environment.NewLine + processResult.errorMessage);
                return;
            }

            _M._currentFormatSet = formatSet;
            OnDocumentProcessed();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var docTypes = _isMsOfficeAvailable ? string.Join(";", FileOperations.AccceptedExtenstions.Select(x => $"*{x}")) : "*.docx";
            openFileDialog.Filter = $"Document files|{docTypes}";
            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                AcceptBtn.IsEnabled = true;
                FileUploadText.Text = _fileName;
            }
        }

        private async void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            //Doc settings
            int age = 0;
            short birthYear = 0;
            if (AnonymousMode)
            {
                if (!short.TryParse(BirthYearText.Text, out birthYear) || !UserProfileRepo.IsBirthYearValid(birthYear))
                {
                    _M._mainWindow.ShowMessage("Ievadiet korektu dzimšanas gadu!");
                    return;
                }
                age = DateTime.Now.Year - birthYear;
            }

            _M._mainWindow.SetLoading(true);
            await Task.Delay(1);
            FormatSet formatSet = AnonymousMode ? Recommendations.GetByAge(age) : UserProfileRepo.GetUserFormatSet();
            
            //Upload
            var uploadResult = await UploadDocument();
            if (!uploadResult)
            {
                _M._mainWindow.SetLoading(false);
                return;
            }

            //Process
            var processResult = await ProcessDocument(formatSet);
            _M._mainWindow.SetLoading(false);
            await Task.Delay(1);
            if (!processResult.result)
            {
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tErrorConvertingDocument"] + Environment.NewLine + processResult.errorMessage);
                return;
            }

            if (AnonymousMode)
            {
                UserProfileRepo.AnonymousBirthYear = birthYear;
            }
            else
            {
                RecentFilesRepo.AddUpdateRecentFile(_fileName, UserProfileRepo.LoggedInUserId);
            }

            _M._currentFormatSet = formatSet;
            OnDocumentProcessed();
        }

        private async Task<bool> UploadDocument()
        {
            if (!FileOperations.IsAllowedFileType(_fileName))
                return false;

            var uploadResult = await Task.Run(() =>
            {
                return FileOperations.UploadNewFile(_fileName);
            });
            if (!uploadResult.result)
            {
                _M._mainWindow.SetLoading(false);
                await Task.Delay(1);
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tErrorAddingNewDocument"] + Environment.NewLine + uploadResult.errorMessage);
                return false;
            }
            return true;
        }

        private async Task<(bool result, bool isPdf, string errorMessage)> ProcessDocument(FormatSet formatSet)
        {
            var isProcessed = await Task.Run(() =>
            {
                var (result, isPdf, errorMessage) = FileOperations.ProcessFile(formatSet);
                return (result, isPdf, errorMessage);
            });
            return isProcessed;
        }

        private bool UpdateRecentFiles()
        {
            try
            {
                var files = RecentFilesRepo.GetRecentFiles(UserProfileRepo.LoggedInUserId);
                HistoryBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    HistoryBox.Children.Clear();
                    foreach (var file in files)
                    {
                        var item = new FileItem(file.FullFileName, new DateTime(file.LastUseTime), OnRecentFileSelected, () => UpdateRecentFiles());
                        HistoryBox.Children.Add(item);
                    }
                }));
                return files?.Count > 0;
            }
            catch (Exception ex)
            {
                ex.TraceEx();
                return false;
            }
        }

        private async void OnRecentFileSelected(string fileName)
        {
            if (!File.Exists(fileName))
            {
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tDocumentNotAvailable"]);
                RecentFilesRepo.RemoveRecentFile(fileName, UserProfileRepo.LoggedInUserId);
                UpdateRecentFiles();
                return;
            }

            _fileName = fileName;
            RecentFilesRepo.AddUpdateRecentFile(_fileName, UserProfileRepo.LoggedInUserId);

            _M._mainWindow.SetLoading(true);
            await Task.Delay(1);
            FormatSet formatSet = UserProfileRepo.GetUserFormatSet();
            var isUploaded = await Task.Run(() =>
            {
                return FileOperations.UploadNewFile(_fileName);
            });
            if (!isUploaded.result)
            {
                _M._mainWindow.SetLoading(false);
                await Task.Delay(1);
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tErrorAddingSelectedDocument"] + Environment.NewLine + isUploaded.errorMessage);
                return;
            }
            var isProcessed = await Task.Run(() =>
            {
                return FileOperations.ProcessFile(formatSet);
            });
            _M._mainWindow.SetLoading(false);
            await Task.Delay(1);

            if (isProcessed.result)
            {
                _M._currentFormatSet = formatSet;
                OnDocumentProcessed();
            }
            else
            {
                _M._mainWindow.ShowMessage((string)App.Current.Resources["tErrorConvertingDocument"] + Environment.NewLine + isProcessed.errorMessage);
            }
        }

        private void OnDocumentProcessed()
        {
            if (_isMsOfficeAvailable)
            {
                _M._mainWindow.SetPage(EPages.Preview);
                return;
            }

            if (DataContext is FileUploadVM dataContext)
            {
                dataContext.IsLimitedBoxVisible = true;
            }

        }

        private void ProcessedOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            DocController.TryOpenDocx(FileOperations.DocxPath);
        }

        private async void ProcessedSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            await DocController.SaveDocument(FileOperations.DocxPath);
        }

        private void ProcessedCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FileUploadVM dataContext)
            {
                dataContext.IsLimitedBoxVisible = false;
            }
        }
    }
}
