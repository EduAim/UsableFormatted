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
using UsableFormatted.Model;
using UsableFormatted.Repos;
using UsableFormatted.ViewModel;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for ProfileBox.xaml
    /// </summary>
    public partial class ProfileBox : UserControl
    {
        private ProfileBoxVM _dataContext;

        public ProfileBox()
        {
            InitializeComponent();
            _dataContext = (ProfileBoxVM)DataContext;
            IsVisibleChanged += ProfileBox_IsVisibleChanged;
        }

        public void UpdateControl()
        {
            UpdateUserElements();
        }

        private void ProfileBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            UpdateUserElements();
        }

        private void UpdateUserElements()
        {
            _dataContext.IsLoggedIn = UserProfileRepo.LoggedInUserId > 0;
            UserIcon.Visibility = _dataContext.IsAuthorizedVisible;
            AnonymousIcon.Visibility = _dataContext.IsAnonymousVisible;

            LoggedInUserName.Text = _dataContext.IsLoggedIn ? (
                string.IsNullOrEmpty(UserProfileRepo.LoggedInUser?.FullName)
                    ? (string.IsNullOrEmpty(UserProfileRepo.LoggedInUser?.Email)
                        ? "Lietotājs" : UserProfileRepo.LoggedInUser.Email)
                    : UserProfileRepo.LoggedInUser.FullName
                ) : "Neautorizēts lietotājs";
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetPage(EPages.LoginView);
            _M._loginView.SetBox("LoginBox");
        }

        private void MenuRegister_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetPage(EPages.LoginView);
            _M._loginView.SetBox("RegisterBox");
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetUserProfile(true);
        }

        private void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            UserProfileRepo.LogoutUser();
            _M._mainWindow.SetPage(EPages.LoginView);
            _M._loginView.SetBox("ButtonBox");
        }

        private void UserIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                UserProfileMenu.IsOpen = true;
            }
        }
    }
}
