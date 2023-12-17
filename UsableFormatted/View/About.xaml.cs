using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
using UsfoModels;

namespace UsableFormatted.View
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            Loaded += About_Loaded;
        }

        private void About_Loaded(object sender, RoutedEventArgs e)
        {
            HomepageUrl.RequestNavigate += HomepageUrl_RequestNavigate;
            VersionText.Dispatcher.BeginInvoke(new Action(() =>
            {
                VersionText.Text = Assembly.GetExecutingAssembly().GetName()?.Version?.ToString() ?? "2";
            }));
        }

        private void HomepageUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true
            });
        }

        private void CloseWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            _M._mainWindow.AboutView.Visibility = Visibility.Collapsed;
        }
    }
}
