using UsableFormatted.Model;
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
    /// Interaction logic for UserListItem.xaml
    /// </summary>
    public partial class UserListItem : UserControl
    {
        private readonly UserProfile _userProfile;
        private readonly Action<UserProfile> _callback;

        //public string Email { get { return _userProfile?.Email ?? "--"; } }
        public string Email => _userProfile?.Email ?? "--";

        public UserListItem(UserProfile userProfile, Action<UserProfile> callback)
        {
            _userProfile = userProfile;
            _callback = callback;
            InitializeComponent();
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(_userProfile);
        }

    }
}
