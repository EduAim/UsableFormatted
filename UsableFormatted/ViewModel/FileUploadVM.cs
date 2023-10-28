using System;
using System.ComponentModel;
using System.Windows;

namespace UsableFormatted.ViewModel
{
    internal class FileUploadVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _IsAnonymousVisible = false;

        public bool IsAnonymousVisible
        {
            get { return _IsAnonymousVisible; }
            set
            {
                _IsAnonymousVisible = value;
                RaisePropertyChanged(nameof(IsAnonymousVisible));
                RaisePropertyChanged(nameof(IsAnonymousElementVisible));
            }
        }

        public Visibility IsAnonymousElementVisible
        {
            get { return _IsAnonymousVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        private bool _isHistory = false;

        public Visibility HistoryPanelVisibility
        {
            get { return _isHistory ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Thickness HistoryMargin
        {
            get { return new Thickness(_isHistory ? 200d : 0, 0, 0, 0); }
        }

        public bool IsHistory
        {
            get { return _isHistory; }
            set
            {
                _isHistory = value;
                RaisePropertyChanged(nameof(IsHistory));
                RaisePropertyChanged(nameof(HistoryPanelVisibility));
                RaisePropertyChanged(nameof(HistoryMargin));
            }
        }

        private bool _isLimitedVisibility = false;

        public Visibility LimitedVisibility
        {
            get { return _isLimitedVisibility ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsLimitedVisibility
        {
            get { return _isLimitedVisibility; }
            set
            {
                _isLimitedVisibility = value;
                RaisePropertyChanged(nameof(LimitedVisibility));
                RaisePropertyChanged(nameof(IsLimitedVisibility));
            }
        }

        private bool _isLimitedBoxVisible = false;

        public bool IsLimitedBoxVisible
        {
            get { return _isLimitedBoxVisible; }
            set
            {
                _isLimitedBoxVisible = value;
                RaisePropertyChanged(nameof(IsLimitedBoxVisible));
                RaisePropertyChanged(nameof(LimitedBoxVisibility));
            }
        }

        public Visibility LimitedBoxVisibility
        {
            get { return _isLimitedBoxVisible ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}