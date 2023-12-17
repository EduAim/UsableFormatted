using UsableFormatted.Model;
using UsableFormatted.Repos;
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

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private List<UserProfile> _allUsers = new List<UserProfile>();

        public LoginView()
        {
            InitializeComponent();
            _M._loginView = this;
            IsVisibleChanged += LoginView_IsVisibleChanged;
        }

        private void LoginView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            UserProfileRepo.LogoutUser();
        }

        private void NoLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.SetPage(EPages.FileUpload);
        }

        //Button box
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            SetBox("RegisterBox");
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            SetBox("LoginBox");
        }

        //Register box
        private void RegRegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            var email = RegEmailText.Text;
            var password = RegPasswordText.Password;
            if (password != RegConfirmPasswordText.Password)
            {
                _M._mainWindow.ShowMessage((string)FindResource("tPasswordsDontMatch"));
                return;
            }

            if (!UserProfileRepo.IsValidEmail(email))
            {
                _M._mainWindow.ShowMessage((string)FindResource("tEmailIncorrect"));
                return;
            }

            if (!short.TryParse(BirthYearText.Text.Trim(), out var birthYear) || !UserProfileRepo.IsBirthYearValid(birthYear))
            {
                _M._mainWindow.ShowMessage((string)FindResource("tEnterCorrectYear"));
                return;
            }

            var created = UserProfileRepo.CreateUser(email, password, birthYear);
            if (!created)
            {
                _M._mainWindow.ShowMessage((string)FindResource("tUserEmailExists"));
                return;
            }
            var login = UserProfileRepo.LoginUser(email, password);
            if (login)
            {
                _M._mainWindow.SetPage(EPages.FileUpload);
                _M._mainWindow.SetUserProfile(true);
            } 
            else
            {
                SetBox("ButtonBox");
            }
        }

        private void RegCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            SetBox("ButtonBox");
        }

        //Login box
        private void LoginLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var email = LoginEmailText.Text;
            var password = LoginPasswordText.Password;
            var login = UserProfileRepo.LoginUser(email, password);
            if (!login)
            {
                _M._mainWindow.ShowMessage((string)FindResource("tIncorrectMailPass"));
                return;
            }
            _M._mainWindow.SetPage(EPages.FileUpload);
        }

        private void LoginCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            SetBox("ButtonBox");
        }

        //Rest
        internal void SetBox(string boxName)
        {
            var boxes = new Dictionary<string, Panel>
            {
                { "ButtonBox", ButtonBox },
                { "RegisterBox", RegisterBox },
                { "LoginBox", LoginBox },
            };

            foreach (var box in boxes)
            {
                box.Value.Visibility = box.Key == boxName ? Visibility.Visible : Visibility.Collapsed;
            }

            if (boxName == "RegisterBox")
            {
                RegEmailText.Text = string.Empty;
                RegPasswordText.Password = string.Empty;
                RegConfirmPasswordText.Password = string.Empty;
                BirthYearText.Text = UserProfileRepo.AnonymousBirthYear > 0 ? UserProfileRepo.AnonymousBirthYear.ToString() : string.Empty;
            }

            if (boxName == "LoginBox")
            {
                LoginEmailText.Text = string.Empty;
                LoginPasswordText.Password = string.Empty;
                FillUserList();
            }
        }

        private void FillUserList()
        {
            _allUsers = UserProfileRepo.GetUsers();
            AvailableUserBox.Children.Clear();
            foreach (var user in _allUsers.OrderBy(x => x.Email))
            {
                var item = new UserListItem(user, ProfileClicked);
                item.Tag = user.Id;
                AvailableUserBox.Children.Add(item);
            }

        }

        private void ProfileClicked(UserProfile userProfile)
        {
            var login = UserProfileRepo.LoginUser(userProfile.Email, skipPassword: true);
            if (login)
                _M._mainWindow.SetPage(EPages.FileUpload);
            else
                _M._mainWindow.ShowMessage((string)FindResource("tAuthError"));
        }

    }
}
