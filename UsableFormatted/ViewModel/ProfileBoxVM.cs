using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UsableFormatted.ViewModel
{
    class ProfileBoxVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isLoggedIn = false;

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                RaisePropertyChanged(nameof(IsLoggedIn));
                RaisePropertyChanged(nameof(IsAnonymousMode));
                RaisePropertyChanged(nameof(IsAuthorizedVisible));
                RaisePropertyChanged(nameof(IsAnonymousVisible));
            }
        }

        public bool IsAnonymousMode { get { return !IsLoggedIn; } }
        public Visibility IsAuthorizedVisible => IsLoggedIn ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAnonymousVisible => IsLoggedIn ? Visibility.Collapsed : Visibility.Visible;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
