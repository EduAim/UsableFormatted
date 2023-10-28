using System;
using System.Collections.Generic;
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
using UsableFormatted.Repos;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for FileItem.xaml
    /// </summary>
    public partial class FileItem : UserControl
    {
        private string _fullFileName;
        private string _fileName;
        private string _filePath;
        private string _lastUseTime;
        private Action<string> _selectedCallback;
        private Action _refreshCallback;

        public string FileName => _fileName;
        public string FilePath => _filePath;
        public string LastUseTime => _lastUseTime;



        public FileItem(string filePathName, DateTime lastTime, Action<string> selectedCallback, Action refreshCallback)
        {
            _fullFileName = filePathName;
            _fileName = System.IO.Path.GetFileName(filePathName);
            _filePath = filePathName[..^_fileName.Length]; //Substring(0, filePathName.Length - _fileName.Length);
            _lastUseTime = lastTime.ToString("dd.MM.yyyy HH.mm");
            _selectedCallback = selectedCallback;
            _refreshCallback = refreshCallback;
            InitializeComponent();
        }

        private void FileBtn_Click(object sender, RoutedEventArgs e)
        {
            _selectedCallback.Invoke(_fullFileName);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            _selectedCallback.Invoke(_fullFileName);
        }

        private void MenuRemove_Click(object sender, RoutedEventArgs e)
        {
            RecentFilesRepo.RemoveRecentFile(_fullFileName, UserProfileRepo.LoggedInUserId);
            _refreshCallback.Invoke();
        }
    }
}
