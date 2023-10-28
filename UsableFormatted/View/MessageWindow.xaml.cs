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
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : UserControl
    {
        private string _message;
        private Action _onClose;

        public MessageWindow(string message, Action onClose)
        {
            InitializeComponent();
            _message = message;
            _onClose = onClose;
            MessageText.Text = _message;
        }

        private void CloseWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            _onClose();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            _onClose();
        }
    }
}
