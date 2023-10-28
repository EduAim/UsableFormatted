using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsableFormatted.Repos;
using UsableFormatted.ViewModel;
using UsfoModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
using Realms.Sync;
using UsfoModels.Model;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for DocPreview.xaml
    /// </summary>
    public partial class DocPreview : UserControl
    {
        public bool IsLoadingVisible { get; set; } = true;
        public bool IsPdfVisible { get; set; } = false;

        public string PdfPath => FileOperations.PdfPath;

        private double _zoom = 1;

        public DocPreview()
        {
            InitializeComponent();
            Loaded += DocPreview_Loaded;
            IsVisibleChanged += AnDocPreview_IsVisibleChanged;
        }

        private void DocPreview_Loaded(object sender, RoutedEventArgs e)
        {
            FontsComboBox.ItemsSource = DocumentParams.Fonts;
            FontSizeComboBox.ItemsSource = DocumentParams.FontSizes;
            HeadingFontSizeComboBox.ItemsSource = DocumentParams.FontSizes;
        }

        private void AnDocPreview_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            if (DataContext is DocPreviewVM dataContext)
            {
                dataContext.IsAuthorized = UserProfileRepo.IsLoggedIn;
            }
            ProfileBox.UpdateControl();

            Zoom = 1;
            DisplayDocument();
        }

        private void DisplayDocument()
        {
            if (DataContext is DocPreviewVM dataContext)
            {
                dataContext.PfdFileName = FileOperations.PdfPath;
                dataContext.Timestamp = DateTime.UtcNow.Ticks.ToString();
            }

            PdfViewerScroll.ScrollToTop();
        }

        private double Zoom { 
            get { return _zoom; }
            set { 
                _zoom = value;
                ItemsScaleTransform.ScaleX = _zoom;
                ItemsScaleTransform.ScaleY = _zoom;
            }
        }

        private void OpenSettings()
        {
            if (DataContext is DocPreviewVM dataContext)
            {
                dataContext.IsSettingsVisible = true;
            }

            var index = Array.IndexOf(DocumentParams.Fonts, _M._currentFormatSet.Font);
            FontsComboBox.SelectedIndex = Math.Max(index, 0);

            index = Array.IndexOf(DocumentParams.FontSizes, _M._currentFormatSet.FontSize.ToString());
            FontSizeComboBox.SelectedIndex = Math.Max(index, 0);

            index = Array.IndexOf(DocumentParams.FontSizes, _M._currentFormatSet.HeadingFontSize.ToString());
            HeadingFontSizeComboBox.SelectedIndex = Math.Max(index, 0);
        }

        private void CloseSettings()
        {
            if (DataContext is DocPreviewVM dataContext)
            {
                dataContext.IsSettingsVisible = false;
            }
        }

        private void ReadSettings()
        {
            _M._currentFormatSet.Font = FontsComboBox.Text;
            decimal decVal;
            if (decimal.TryParse(FontSizeComboBox.Text, out decVal))
                _M._currentFormatSet.FontSize = decVal;

            if (decimal.TryParse(HeadingFontSizeComboBox.Text, out decVal))
                _M._currentFormatSet.HeadingFontSize = decVal;

        }

        private async void ReprocessDocument()
        {
            _M._mainWindow.SetLoading(true);
            await Task.Delay(1);
            var isProcessed = await Task.Run(() =>
            {
                return FileOperations.ProcessFile(_M._currentFormatSet);
            });
            _M._mainWindow.SetLoading(false);
            await Task.Delay(1);

            if (isProcessed.result)
            {
                DisplayDocument();
            }
            else
            {
                _M._mainWindow.ShowMessage("Kļūda pārveidojot dokumentu!" + Environment.NewLine + isProcessed.errorMessage);
            }
        }

        private void UpdateUserSettings()
        {
            UserProfileRepo.UpdateUserFormat(UserProfileRepo.LoggedInUserId, _M._currentFormatSet);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            DocController.TryOpenDocx(FileOperations.DocxPath);
        }

        private void OpenPdfButton_Click(object sender, RoutedEventArgs e)
        {
            DocController.TryOpenPdf(FileOperations.PdfPath);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserProfileRepo.IsLoggedIn)
                _M._mainWindow.SetSurvey(true);
            else
                _M._mainWindow.SetPage(EPages.LoginView);
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            await DocController.SaveDocument(FileOperations.DocxPath);
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Microsoft Word document (*.docx)|*.docx";
            //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                _M._mainWindow.SetLoading(true);
                var isSaved = await Task.Run(async () =>
                {
                    try
                    {
                        var bytes = await File.ReadAllBytesAsync(FileOperations.DocxPath);
                        await File.WriteAllBytesAsync(saveFileDialog.FileName, bytes);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        ex.TraceEx();
                        return false;
                    }
                });
                _M._mainWindow.SetLoading(false);
            }
            //*/
        }

        private async void SavePdfFileButton_Click(object sender, RoutedEventArgs e)
        {
            await DocController.SaveDocument(FileOperations.PdfPath);
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Portable Document Format (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                _M._mainWindow.SetLoading(true);
                var isSaved = await Task.Run(async () =>
                {
                    try
                    {
                        var bytes = await File.ReadAllBytesAsync(FileOperations.PdfPath);
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
            }
            //*/
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void Plus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Plus_MouseDown();
        }

        private void Minus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Minus_MouseDown();
        }

        private void Reset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Reset_MouseDown();
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Plus_MouseDown()
        {
            var newZoom = Math.Round(Zoom + 0.1, 1);
            if (newZoom <= 5)
                Zoom = newZoom;
            Debug.WriteLine($"Plus {Zoom}");
        }

        private void Minus_MouseDown()
        {
            var newZoom = Math.Round(Zoom - 0.1, 1);
            if (newZoom > 0)
                Zoom = newZoom;
            Debug.WriteLine($"Minus {Zoom}");
        }

        private void Reset_MouseDown()
        {
            Zoom = 1;
            Debug.WriteLine($"Reset {Zoom}");
        }

        private void SettingsAplyBtn_Click(object sender, RoutedEventArgs e)
        {
            ReadSettings();
            CloseSettings();
            ReprocessDocument();
        }

        private void SettingsSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            ReadSettings();
            CloseSettings();
            UpdateUserSettings();
            ReprocessDocument();
        }

        private void SettingsCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseSettings();
        }
    }
}
