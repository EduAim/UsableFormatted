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
            if (UserProfileRepo.LoggedInUserId > 0)
            {
                SetPage(EPages.FileUpload);
            }
            MoodleController.StartSocketServer();
            MoodleController.OnDocumentReceived += MoodleController_DocumentReceived;
        }

        private void MoodleController_DocumentReceived(object sender, EventArgs e)
        {
            string phrase = "default";
            if (sender is string)
            {
                phrase = (string)sender;
            }
            Debug.WriteLine($"..MoodleController_DocumentReceived: {phrase}");
        }

        public void SetPage(EPages page)
        {
            Dictionary<EPages, UserControl> controlDict = new Dictionary<EPages, UserControl>{
                { EPages.LoginView, ViewLoginView},
                { EPages.FileUpload, ViewFileUpload},
                { EPages.Preview, ViewDocPreview },
            };

            foreach (var control in controlDict)
            {
                control.Value.Visibility = control.Key == page ? Visibility.Visible : Visibility.Collapsed;
            }
            MainBackground.Color = page == EPages.LoginView ? (Color)ColorConverter.ConvertFromString("#ff235b8e") : Colors.White;
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

        private void MenuHome_Click(object sender, RoutedEventArgs e)
        {
            AboutView.Visibility = Visibility.Collapsed;
            SetPage(EPages.LoginView);
            _M._loginView.SetBox("ButtonBox");
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutView.Visibility = Visibility.Visible;
        }

    }
}
