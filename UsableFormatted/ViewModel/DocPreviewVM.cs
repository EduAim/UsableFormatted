using System.ComponentModel;
using System.Windows;

namespace UsableFormatted.ViewModel
{
    internal class DocPreviewVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _pdfFileName = string.Empty;

        public string PfdFileName
        {
            get { return _pdfFileName; }
            set
            {
                _pdfFileName = value;
                RaisePropertyChanged(nameof(PfdFileName));
            }
        }

        private string _timestamp = string.Empty;

        public string Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
                RaisePropertyChanged(nameof(Timestamp));
            }
        }

        private bool _isAuthorized = false;

        public bool IsAuthorized
        {
            get
            {
                return _isAuthorized;
            }
            set
            {
                _isAuthorized = value;
                RaisePropertyChanged(nameof(IsAuthorized));
                RaisePropertyChanged(nameof(IsAnonymousElementVisible));
                RaisePropertyChanged(nameof(IsAuthorizedElementVisibility));
            }
        }

        public Visibility IsAnonymousElementVisible
        {
            get { return _isAuthorized ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility IsAuthorizedElementVisibility
        {
            get { return _isAuthorized ? Visibility.Visible : Visibility.Collapsed; }
        }

        private bool _isSettingsVisible = false;

        public bool IsSettingsVisible
        {
            get
            {
                return _isSettingsVisible;
            }
            set
            {
                _isSettingsVisible = value;
                RaisePropertyChanged(nameof(IsSettingsVisible));
                RaisePropertyChanged(nameof(SettingsVisibility));
            }
        }

        public Visibility SettingsVisibility
        {
            get { return _isSettingsVisible ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}