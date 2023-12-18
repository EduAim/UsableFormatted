using UsableFormatted.Controller;
using UsableFormatted.Model;
using UsableFormatted.Repos;
using UsableFormatted.View;
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
using System.Diagnostics;
using System.Timers;

namespace UsableFormatted
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isLoading = false;
        private const string LANGUAGE_EN = "en";
        private const string LANGUAGE_LV = "lv";
        private GlobalSettings _settings;

        public MainWindow()
        {
            InitializeComponent();
            _M._mainWindow = this;
            Loaded += MainWindow_Loaded;
            Title = "Usable & Formatted";
            SetPage(EPages.LoginView);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _settings = GlobalSettingsRepo.GetSettings();
            SetLanguage(_settings.Language);
            UserProfileRepo.OnUserChanged += UserProfileRepo_OnUserChanged;
            if (UserProfileRepo.LoggedInUserId > 0)
            {
                SetPage(EPages.FileUpload);
            }
            UpdateMenus();
            var isMoodle = MoodleController.InitSocketServer();
            if (isMoodle)
            {
                MoodleController.OnDocumentReceived += MoodleController_DocumentReceived;
            }
        }

        private void UserProfileRepo_OnUserChanged(object sender, EventArgs e)
        {
            UpdateMenus();
        }

        private void MoodleController_DocumentReceived(object sender, EventArgs e)
        {
            if (!(sender is string phrase) || string.IsNullOrEmpty(phrase))
                return;
            
            FileOperations.AutoLaunchFileName = phrase;
            SetWindowActive();
            if (ViewFileUpload.Visibility == Visibility.Visible)
            {
                ViewFileUpload.DocumentAutoLaunch();
            }
            else
            {
                SetPage(EPages.FileUpload);
            }
        }

        public void SetWindowActive()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!this.IsVisible)
                    this.Show();

                if (this.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Normal;

                this.Activate();
            }));
        }

        public void SetPage(EPages page)
        {
            Dictionary<EPages, UserControl> controlDict = new Dictionary<EPages, UserControl>{
                { EPages.LoginView, ViewLoginView},
                { EPages.FileUpload, ViewFileUpload},
                { EPages.Preview, ViewDocPreview },
            };

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var control in controlDict)
                {
                    control.Value.Visibility = control.Key == page ? Visibility.Visible : Visibility.Collapsed;
                }
                MainBackground.Color = page == EPages.LoginView ? (Color)ColorConverter.ConvertFromString("#ff235b8e") : Colors.White;
            }));
        }

        public void SetSurvey(bool isVisible)
        {
            SurveyView.Dispatcher.BeginInvoke(new Action(() =>
            {
                SurveyView.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        public void SetUserProfile(bool isVisible)
        {
            UserSettingsView.Dispatcher.BeginInvoke(new Action(() =>
            {
                UserSettingsView.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        public void SetLoading(bool isLoadingOn)
        {
            _isLoading = isLoadingOn;
            ViewLoading.Dispatcher.BeginInvoke(new Action(() =>
            {
                ViewLoading.Visibility = _isLoading ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        public void ShowMessage(string message)
        {
            MessageWindow? box = null;
            box = new MessageWindow(message, () => {
                box.Visibility = Visibility.Collapsed;
                MessageContainer.Children.Remove(box);
            });
            MessageContainer.Children.Add(box);
        }

        private void SetLanguage(string language = LANGUAGE_LV)
        {
            ((App)Application.Current).ChangeAppLanguage(language);
            foreach (MenuItem menuItem in LangMenu.Items)
            {
                menuItem.IsChecked = language == menuItem.Tag as string;
            }
        }

        private void SaveApplyLanguage(string language)
        {
            _settings.Language = language;
            GlobalSettingsRepo.SaveSettings(_settings);
            SetLanguage(language);
        }

        private void UpdateMenus()
        {
            var isLoggedIn = UserProfileRepo.IsLoggedIn;
            MenuLogin.IsEnabled = !isLoggedIn;
            MenuLogout.IsEnabled = isLoggedIn;
            MenuRegister.IsEnabled = !isLoggedIn;
            MenuSettings.IsEnabled = isLoggedIn;
        }

        private void MenuHome_Click(object sender, RoutedEventArgs e)
        {
            AboutView.Visibility = Visibility.Collapsed;
            if (UserProfileRepo.IsLoggedIn)
            {
                SetPage(EPages.FileUpload);
            } 
            else
            {
                SetPage(EPages.LoginView);
                _M._loginView.SetBox("ButtonBox");
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutView.Visibility = Visibility.Visible;
        }

        private void MenuLangEn_Click(object sender, RoutedEventArgs e)
        {
            SaveApplyLanguage(LANGUAGE_EN);
        }

        private void MenuLangLv_Click(object sender, RoutedEventArgs e)
        {
            SaveApplyLanguage(LANGUAGE_LV);
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            SetPage(EPages.LoginView);
            _M._loginView.SetBox("LoginBox");
        }

        private void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            UserProfileRepo.LogoutUser();
            SetPage(EPages.LoginView);
            _M._loginView.SetBox("ButtonBox");
        }

        private void MenuRegister_Click(object sender, RoutedEventArgs e)
        {
            SetPage(EPages.LoginView);
            _M._loginView.SetBox("RegisterBox");
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            SetUserProfile(true);
        }
    }
}
